# Reproduces the key results in "Seal Thermal Margin Analysis.md" (open-air calibration, argon, final design).
# Usage (Windows PowerShell 5.1), from any folder:   powershell -ExecutionPolicy Bypass -File run_examples.ps1
# Needs only the .NET C# compiler that ships with Windows PowerShell (Add-Type). Takes about a minute.

$here = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not ('TM4' -as [type])) { Add-Type -TypeDefinition (Get-Content (Join-Path $here 'plume_model.cs') -Raw) -Language CSharp }

# Output columns returned by [TM4]::Sim(p): each row = [t(s), sampleEnd, sealMid, rod6mmBelowSeal, ceramicTopFace,
#   lidTop, rod1inAboveLid, rod0.5inBelowSeal, rodAtLidBottom, rodEndAboveGap, gasAtLidBottom]   (all temps in C)
function Pick($rows, $t) { $best = $null; $bd = 1e9; foreach ($r in $rows) { $d = [math]::Abs($r[0] - $t); if ($d -lt $bd) { $bd = $d; $best = $r } }; $best }

# Build a parameter set. Defaults = the calibrated open-air model (plume x1.36, 2 W lid heat, joint 45 K/W, whole sample hot).
function New-P($Th = 800, $rodBottomIn = 2.66, $cu = $false, $clamp = $false, $argon = $false, $rClamp = 0.8, $layerInf = 20) {
    $p = New-Object TP4
    $p.wholeSampleHot = $true; $p.jet = $false      # the air jet was tested and is INEFFECTIVE - keep it off
    $p.rJoint = 45; $p.sealQ = 2.0; $p.rodBottom = $rodBottomIn * 0.0254; $p.Th = $Th
    $p.clamp = $clamp; $p.rClamp = $rClamp
    if ($cu) { $p.kRod = 391; $p.rhoRod = 8960; $p.cRod = 385; $p.eRod = 0.15 }   # 110 copper, McMaster 8966K13
    if ($argon) { $p.fc = 1.353; $p.hg = 15.4; $p.dL0 = $layerInf / 2; $p.dLinf = $layerInf; $p.tauL = 3000 } else { $p.fc = 1.36; $p.hg = 22 }
    $p
}
function Run-P($p, $tHold = 750) {
    $q = $p.GetType().GetFields();
    $a = New-Object TP4; foreach ($f in $q) { $f.SetValue($a, $f.GetValue($p)) }; $a.dt = 3; $a.tEnd = $tHold + 1; $a.recEvery = 3
    $b = New-Object TP4; foreach ($f in $q) { $f.SetValue($b, $f.GetValue($p)) }; $b.dt = 20; $b.tEnd = 60000; $b.recEvery = 60000
    $r1 = [TM4]::Sim($a); $r2 = [TM4]::Sim($b); $x = Pick $r1 $tHold; $s = $r2[$r2.Count - 1]
    [pscustomobject]@{ 'LidBottom@750s' = [math]::Round($x[8], 0); 'Seal@750s' = [math]::Round($x[2], 0); SealSteady = [math]::Round($s[2], 0)
        CeramicTopFace = [math]::Round($s[4], 0); RodEndAboveGap = [math]::Round($s[9], 0); 'TC(avg)' = [math]::Round(($s[4] + $s[9]) / 2, 0); GasAtLid = [math]::Round($s[10], 0) }
}

"=== 1. Open-air calibration (targets: sandwiched TC steady ~197 C, rod at lid bottom ~85-90 C at 750 s; R_bottom 2.66 in, 304SS, no cooling) ==="
Run-P (New-P 800 2.66) | Format-Table -AutoSize | Out-String
"=== 2. Planned build (RodBottom 2.103 in), open air vs argon, 800 C ==="
$rows = foreach ($c in @(@('304SS, no cooling', $false, $false), @('110 Cu, no cooling', $true, $false), @('110 Cu + cooling block', $true, $true))) {
    foreach ($gas in $false, $true) { $o = Run-P (New-P 800 2.103 $c[1] $c[2] $gas); $o | Add-Member Case $c[0]; $o | Add-Member Gas $(if ($gas) { 'argon' } else { 'air' }); $o } }
$rows | Select-Object Case, Gas, 'Seal@750s', SealSteady, CeramicTopFace, 'TC(avg)', GasAtLid | Format-Table -AutoSize | Out-String
"=== 3. Final design (110 Cu + cooling block, argon): seal vs block total resistance ==="
$rows = foreach ($Th in 800, 1000) { foreach ($rc in 0.3, 0.8, 2.0, 5.0) { $o = Run-P (New-P $Th 2.103 $true $true $true $rc); $o | Add-Member Sample_C $Th; $o | Add-Member BlockR_KperW $rc; $o } }
$rows | Select-Object Sample_C, BlockR_KperW, SealSteady, CeramicTopFace, 'TC(avg)' | Format-Table -AutoSize | Out-String
