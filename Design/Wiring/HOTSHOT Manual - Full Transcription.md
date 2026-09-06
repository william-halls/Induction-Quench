---
subsystem: wiring
tags: [wiring, electrical, reference, ambrell, hotshot, manual]
related: [[Design/Wiring/Electrical System.md]], [[Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring.md]]
source: media/wiring/Ameritherm HOTSHOT 103927 Manual (Doc 801-9252k).pdf
status: reference
---

# HOTSHOT Manual — Full Text Transcription

Plain-text transcription of the Ameritherm HOTSHOT manual (Doc# 801-9252k.doc, revision K), photographed page-by-page and saved as [[media/wiring/Ameritherm HOTSHOT 103927 Manual (Doc 801-9252k).pdf]]. This note exists so future sessions can read text instead of re-processing 47 photographed pages as images. Page numbers below refer to the manual's own printed page numbers (not the PDF page index, which has a few unnumbered front-matter pages first).

Our unit: **HOTSHOT 7.5kW, SN 103927**, plus handheld remote control module part no. 301-0243D (no manual located for the remote itself).

---

## Table of Contents (as printed)

1. Introduction — 5
   1.1 Safety Considerations — 6
   1.2 Front and Rear Panels — 7
   1.3 Heat Station — 8
   1.4 Assembly — 9
2. How Your HOTSHOT Works — 13
   2.1 Home Zone — 14
   2.2 Control Zone — 20
   2.3 Detailed HOTSHOT Operation — 24
   2.4 Equipment Maintenance — 27
   2.5 Smartburst Technology — 28
3. Customizing Your HOTSHOT — 29
   3.1 Rear Panel Connections — 29
   3.2 Optional Equipment — 40
   3.3 Alarm Limits — 42
   3.4 Tap Adjustment — 43
4. Technical Information — 45
   4.1 Mechanical — 45
   4.2 Electrical — 47 (photographed as manual p.46 in our copy)
   4.3 Environmental — 49
5. Theory of Coil Design — 51 (not photographed)

Derivation note printed on the manual's back cover: "This K-revision has been written to reflect changes to RHS tapping method, hardware." Ameritherm Inc., 39 Main Street, Scottsville, NY 14546. tel 1.800.456.4328, fax 1.585.889.4030, www.ameritherm.com

---

## 1. Introduction

Your HOTSHOT is a solid-state induction heating system that converts three-phase line voltage to a 3.5, 5, or 7.5kW output (depending on model) over a range of radio frequencies (RF) and voltages. This energy is delivered to a remote series-resonant circuit — including your coil — where a precisely controlled magnetic field is created around your work-piece.

HOTSHOT uses a special sensing technology enabling greater control of your process; precise frequency registration under varying load conditions results in increased precision in heat control and times-to-temp.

**Special features:**
- Touch-pad interface for control and programming
- Backlit LCD simultaneous display of up to four data fields
- Electrically isolated remote heat station

**The power from your supply can be controlled:**
- manually from the front panel
- remotely by your signals provided to the rear panel
- remotely through a configurable serial port
- from one of four 5-step profiles you specify

This selection is made from the touch-pad and can be changed at any time.

**Unpacking:** Your Ameritherm HOTSHOT Induction Heating system has been carefully packed to arrive at your facility in good condition. We suggest you inspect the shipping cartons in the presence of the carrier when the unit arrives at your plant. Look for dents, crushed corners or torn cartons.

**YOU MUST REPORT ALL DAMAGE DIRECTLY TO THE CARRIER.** Check to see that all the parts we shipped arrived at your plant:
- HOTSHOT power supply
- remote heat station with cable
- reference coil, attached to mounting blocks
- this manual

### 1.1 Safety Considerations

Your HOTSHOT uses RF energy to raise the temperature of your work-piece. Most of this RF energy is transmitted into the work-piece and some is transmitted into the air.

**Physical Risks** — During the ⚡HeatOn cycle, high RF voltages are present at your work-coil and internal electric currents in your part, which coil currents from the coil heats your part. There are several risks which we instruct you to protect against:
- **High Temperatures**: take steps to prevent personal contact with the work-piece you are heating. Severe burns can result from contact at these temperatures!
- **RF Voltages**: we recommend that you provide protection against personal contact with the work-coil when it is energized (⚡HeatOn).
- **Induction burns**: the energized work-coil causes nearby metals to become heated; DO NOT WEAR JEWELRY OR CARRY METALS within a few inches or centimeters of the energized work-coil.

**Safety Circuits**: Your optional normally-closed E-stop switch can be installed in series with the system 24Vdc supply (CTB1:15-16). When the switch opens, all output power and operations cease. When the switch is re-closed, the power supply returns to ★Ready (non-hazardous state) @ 24Vdc.

The power supply is fitted with an external rear panel jumper (CTB1:15-16) which must be removed to insert any safety switches required.

**⚠ This is the key quote for E-stop scope: the manual describes this purely as an internal 24Vdc control-rail interlock. It never states this loop disconnects mains/chassis power. Treat "fully de-energizes" as unconfirmed for compliance purposes.**

### 1.2 Front and Rear Panels

**Touch-pad & Display** — all user interface is conducted here:
- Status LEDs indicate what your HOTSHOT is doing
- AC indicator is lighted when unit is turned on
- LCD display provides operating information, navigation/programming prompts
- START/STOP buttons are used to start/stop front panel and to program and control the unit
- Navigation buttons are used to program and control from front panel

Front panel labels (left to right): Handles, Display, Status, AC indicator, Navigation, START/STOP.
Rear panel labels (left to right): Mains Inlet, Mains breaker, CTB1, RF Output, System Water, Heat Station Water.

**Mains breaker**: This going switch controls the mains connection to the power supply.
**Mains Inlet**: This strain-relief fitting secures your mains cabling.

### 1.3 / 1.4 Assembly

**Attach heat station & water**
Take these factors into consideration as you determine where to install your system:
- availability of water and drain
- availability of 3φ AC power

Steps: unscrew protective cap → insert un-heated heat station plug → tighten ring nut → insert RED tube from heat station → insert BLU tube from heat station → insert CLEAR tube from water outlet → insert clear tubing for water inlet → fully close cap ring → push in on retaining rings, pull out to remove tubing.

**Heat station mounting base options**: Your heat station has been fitted with two mounting screws for length; add up to 1" to your fixture depth to include the amount of secondary heating you might experience combined. Secured with four M8 nylon screws, this base can be utilized or omitted in your mounting scheme. To mount: remove nylon panel screws, add 1" to your fixture depth to include mounting plate without the plate.

**Work-coil or reference coil (included with the unit)**
A reference coil is included with your heat station to assure basic operation; while it is suitable for use in heating, another coil may have been ordered for your use.

If you are attaching your own coil:
- use 4mm Allen-wrench to remove the 4 screws securing the reference coil
- remove reference coil and retain 2 O-rings from coil mounting plate
- reuse or use new O-rings; insert in recessed face of heat station
- secure coil with 4 screws provided; tighten

**Rack/panel Mounting**
Your HOTSHOT is mountable in your panel or 19" rack (3U), observe these steps and requirements:
- unscrew/remove 4 rubber feet on unit base
- (panel) provide mounting pattern per above; plan for M3 or M6 screws
- ensure that rear panel of unit is suspended when mounted
- provide at least 24" depth to permit water and AC cordage bend
- secure unit with M3 or M6 hardware

Dimensions (panel/rack cutout): 18.3 (465), 17.5 (445), 9.2 (180), 2.25 (57), 1.5 (38) mm in parens.

**Cooling water**
In the process of heating your work-piece, the current flowing through the heat station and its components, too, provides a means of directing the excess heat from your supply and through the components (both non-ferrous); it can lead to serious equipment damage if allowed to be recycled back to the equipment or to cooling water; transfer it, either to drain or to cooling water systems.

Follow these important restrictions:
- If used, fill re-circulating water systems with distilled water
- Do not use de-ionizing crystals as anti-freeze agents
- Do not use unregulated pH cooling systems
- Do not use uninhibited (both non-ferrous) coolers is not sufficient; install by Ameritherm a pressure regulator and a particulate matron pipe; it can lead to serious equipment damage!
- Do not use ferrous tubing

Establish a cooling water source: capable of 40psi (minimum) differential pressure across the system and at least 1.5 gpm (5.7 l/m) flow
- Provide inlet and outlet water through ¾" OD plastic or copper tubing
- Cut each tube end cleanly and remove any burrs
- Insert drain tube end fully into lower fixture labeled OUTLET (?, above)
- Insert supply tube fully into upper fixture labeled INLET (?, above)

(To remove tubing, press retaining ring while pulling on tube.)

**Flow monitoring**
The amount of water flowing through your equipment is monitored, since operation. When flow is inadequate or absent cooling water can result in damage to the coil or its internal switches — two internal switches are closed. When flow falls off to the heat sink or to the coil, one or both of the switches open and indicates ★Fault. The power supply falls off to the heat sink or to the coil, ★Fault.
- No action required by you, internal connections.

### Mains connection

Your HOTSHOT power supply requires 3φ AC power and ground; you provide this service with your cable at the rear of the power supply:
- use Phillips screwdriver, remove 12 cover screws
- remove plastic safety shield by removing screw at center
- select wire gauge from §4.2 current data
- remove ~12" of your 4-conductor cable, strip 4 wires for crimp ring-lugs
- insert 4-crimp ring-lugs through strain-relief fitting and secure with safety marked ground screw — this accepts cables from 0.51 – 0.71 in (13 – 18mm)
- crimp ring-lugs to secure cable
- Mount 4 ring-lugs to A-B-C, replace nut-washer combinations; tighten
- terminals A-B-C
- attach ground wire to terminal safety marked with safety ground symbol
- replace safety shroud; secure with center screw
- replace unit cover; secure with 12 screws

### Start-up display

Upon power-up, certain important information about your HOTSHOT is displayed:

| | |
|---|---|
| HotShot 7.5 | 7.5kw RF Power |
| Display: Main: | Vn.nn Vn.nn |
| System ID #: | 12345P |

Firmware versions. There may be instances where the Display or the version number of the firmware is unique to your unit. You will need this number if you elect to purchase any of a group of feature-unlock codes from Ameritherm.

System ID#: This number is unique to your unit.

You can also view these screens when the unit is ON, by pressing ▲ or ▼ to expand the Start-up display screens on the touch-pad.

§2.3 press ▲ or ▼ to expand what you see when the unit is operating in the Home zone.

The next section explains what you see when the unit is working (working up).

---

## 2. How Your HOTSHOT Works

When you turn on ★HeatOn, next page, this equipment concentrates an electromagnetic field within the coil diameter. When you put your metal part into the coil, this field causes internal electric currents in your part. Friction from these currents heats your part.

With ★HeatOn, both Setpoint (your selected output level) and Output Level (actual power or current delivered to your part) are displayed. Typically, the Output Level is highest when your part is in the coil and it returns to a lower level when you remove your part from the coil (for detailed explanations or to understand operation that is not typical, see §2.3).

⚠ LED Status and system notices are relevant if your **part is in the coil during ★HeatOn during** operation. Status and notices displayed after you remove your part can be ignored.

This equipment is designed to operate simply, requiring you to have very little knowledge of induction. This section presents ★HeatOn operation in two parts:

- **Home** zone: your HOTSHOT operates in this zone when you first apply power. The factory reset simple; it allows you to get right to work heating parts by operating the front panel. From the Home zone, you can adjust output levels by setting the timer and on/off power. Simple heating jobs may never require you to use features outside this zone.
- **Control** zone: this is how you make use of the full set of HOTSHOT capabilities by changing any of the operating characteristics. Refer to this section to customize your HOTSHOT.

(Other operating characteristics are discussed in the Control zone section.)

Next is a simple example of operation from the Home zone. In it you will operate your HOTSHOT by:
- starting from the front panel
- using 2-button START (requires 2-button start type)

After this example, the front panel display, LED and touch-pad features are explained. The Control zone is explained in detail in the next section.

### 2.1 Home Zone

When you turn it on, HOTSHOT operates in the **Home zone** (illustration). Within this zone, both experienced and inexperienced users, setting the timer and turn RF power on and off:

**Home Zone Display** shows: Timer, Setpoint/Output, Freq

**Timer**: cycle or continuous. Time adjustments are made with 3 wires (X, Y and jumper Z), which are attached in a 3x3 terminal block.

**Status LEDs** indicate what your HOTSHOT is doing:
- READY: waiting to heat, no faults detected
- HEAT ON: power is being delivered
- FAULT: heating interrupted, fault detected

**AC Power**: indicates your HOTSHOT is turned on
**Navigation**: to program and configure from front panel

**Simplest example**: In this example, you heat the part as shown, at 100A output.

**Do this (untimed):**
1. open water valve, assure water is flowing
2. check the work-coil
3. check condition of (any) safety barriers or limit switches; all safety devices are clear, electrically closed
4. turn on HOTSHOT — AC Power and Ready LEDs light
5. use display, adjust Setpoint: assure 100A
6. press ▲ adjust until 100A
7. secure unheated part in coil
8. press START
9. press STOP
10. remove heated part from coil

**Observe**: check water flow; assure there is no work-piece yet; check safety barriers closed; AC Power and Ready LEDs; assure Setpoint 100A; part not contacting coil, do not hold; ★HeatOn, ★Ready; ★Ready only; avoid contact with heated portions.

**Timed heating example (10.00s):**
1. select ▲ Timer
2. adjust ▲ until 10.00s
3. secure unheated part in coil
4. press START

Button is used to move data field from one data field to the next. STOP halts induction heating (Home zone only). START begins heating; ready signal returns when heat cycle completes (heating). STOP signal from front or rear panel always halts heating.

**Display features:**
- **Setpoint/Output**: at the top left corner of the screen, this is your desired output level, displayed in RF Amps. With the pointer (▶) on Setpoint, you can increase or decrease this setting by values between 30 milliamps and 999 amps. To quickly reset the Timer to 0:00 if indicated (▶).
- **Timer/Freq**: at the bottom right corner of the screen, the frequency is displayed at the actual current frequency of heating; this is the RF output frequency of your coil in combination with the heat coil (turned off). When the output power is turned on, this displays the output level in RF Amps or RF Watts.
- **Timer** works to match the RF output to this setting when the RF output is on (assure 2button Start ★1.2 when heating starts). At top right, use the timer to control the duration of your heating. Larger values result in faster heating or hotter parts. With the pointer (▶) on Setpoint, you can increase or decrease the setting with the ▲/▼ buttons. Setpoint on Timer, you can increase or decrease values between 30 milliamps and 999 amps. At top right, use the timer to control duration; elapsed cycle time is then displayed during Untimed (heatOn) and 9999 seconds when the pointer starts to count down when you press START, then press STOP.

At the bottom left corner of the screen, the frequency at which your coil is heating is shown. During the first heat cycle (★HeatOn), the frequency value shows what the frequency between heating cycles/loads. (You cannot set frequency or current — this cannot exceed your frequency range and coil selection.) The output frequency depends on the recent heating cycle. Sometimes, you may see the output frequency approaches either end of the operation range of HOTSHOT; if this happens and we will supply you with the appropriate advice for assistance. Please call Ameritherm for assistance should this happen and we will supply you with the appropriate capacitors.

### 2.2 Control Zone

This is how you preview or change important operating characteristics of the HOTSHOT Control Zone is accessible only while heat is off:
- From the Home zone, press [Home button icon], the Control Zone menu is shown. Use ▲/▼ to select and press ▶ to select (safe mode only when timed).

**Menu Selection** ▶System Options

**Control From**:
- Front Panel — HeatOn/Timing controlled at front panel
- Rear Panel — HeatOn/Timing signaled by opto-isolated inputs at rear panel; can control from any of four profiles you specify
- File A...D — one of four heat station profiles you specify
- RS485 4-wire — if Communications = RS485 4 (Communications settings)

**Start Type**:
- 2button — HeatOn only starts when STOP and remains until STOP signal (whenever normally closed STOP signal opens)
- 1button — HeatOn only starts and remains until STOP signal (jumpered) or when signal opens

**Display Percent**:
- Off — not used
- On — in use
- Adjust ▲/▼ — Alarm Limits for instructions (see §3.3)

**Alarm Limits**
- Off/if not used
- On — in use
- Adjust
- see §3.3 Alarm Limits for instructions

**Lock Front Panel**
To prevent unauthorized changes to settings:
- No: disables all buttons except START, STOP
- Yes: select and follow instructions, enter secret unlock code (or RS485 4-wire)
To unlock, press ▲▼ Alarm Limits for instructions (or, locking is prevented)

**Contrast Adjust**
To change the contrast of the displayed characters (lower number = lighter characters). Use ▲ then ▼ to adjust; use ▲▼ to save setting (locking is retained)

**Communication**
- None
- RS4854 Wire (only protocol at this setting)
Use ▲ to select, then ▲ to enable (if RS485 4-Wire). See §3 Serial Communication

**Auxiliary Out**
- Disabled — feature not supported at this time

**Language Choice**
- English — select from any of the available languages

**Start From**
- Front — Front panel button to START RF heating from the front panel
- Rear — front panel does not do the STOP; (NC relays, decrypt CTB1) if the rear end signal
- START with 4-Wire "Communication = RS485 4 Wire"; follow relayed command (STOP command) serial or rear...

**System Status** (Menu Selection ▶System Status)
Once this screen is displayed, press ▲/▼ to select and press ▶ to select from a previous Fault notices.
Displays the temperature of active components on the heat sink (HS) and the internal ambient air (AM) and any active (unresolved) Fault notices (§2.3).
- press ▲/▼ to display any previous Fault notices (from last heat cycle)
- use ▲/▼ to change between °F and °C

**Adjust Profiles** (Menu Selection ▶Adjust Profiles)
This lets you build your independent sequenced-heating profiles (A-D); you set the starting and ending output levels and the duration for as many as five discrete steps per Profile (e.g. A1-A5). Multi-step, following sections)

**Select Profile** ▶A B C D

**Heat Station** (Menu Selection ▶Heat Station)
This enables the power supply to operate after you make changes to the heat station or if you switch from one heat station to another heat station. (Factory settings need never be changed if you make no changes to the heat station.)
To change, press ▶ for changes to either capacitor or to transformer tap settings.

Although factory-set, you must inform the HOTSHOT of any changes you make to the transformer taps or tank capacitor changing instructions (§3.4); you must inform the power supply of tap and capacitor changing instructions.

**Choose Menu** ▶Modify Tap
**Choose Menu** ▶Modify Cap
**Choose Model** ▶150
Cl value: 0.10uF
**Select Tap** ▶18

(This information provided by Ameritherm.)

⚠ If you have changed the transformer tap settings following a change in your application: (§3.4) you must inform the power supply. Beginning from the highest tap and Cap protection. Failure to change tap or Cap settings following a change may result in false displays of component protection. This does not change capacitance.

⚠ Whenever you are unsure about the correct tap selection for your application, follow this rule: **start high, work down.** Beginning from the highest transformer tap presents the least stress to the HOTSHOT.

### 2.3 Detailed HOTSHOT Operation

This section details how to:
- interpret the Status LEDs
- understand system Faults and Advisories
- program and execute continuous and multi-step heat-cycles (profiles)

**Interpret Status LEDs**
This guide will help you understand the operation of your HOTSHOT.

⚠ LED Status and system notices are relevant if your **part is in the coil during ★HeatOn during** operation. Status and notices displayed after you remove your part can be ignored.

**Ready**: HOTSHOT is waiting to heat; no faults detected

**Heating**: START command received, outputting power per settings. It may be possible to achieve a more efficient configuration, though output is within acceptable margins. See §3 for possible suggestion to improve operation, refer to §3 for further adjusting instructions.

**Limit**: Unit heating, not meeting setpoint. ⓵display: output differs from setpoint by more than 2%. L* ⓶display: pre-set Alarm Limit has been exceeded. ⓷ for possible corrective step(s). ...W (overcurrent): displayed output power is unable to reach a preferred tolerance band. Increase tap for possible improvements.

**Icap** ⓸display: you are attempting to adjust Setpoint above safe limit for capacitors used. Reduce Setpoint.

**Fault**: Heating interrupted, fault detected, correction required. ⓹ to Control zone ▶Status then ⓺ for fault notice

★AC Power: indicates HOTSHOT is ON. ⓻ see the Status screen for notices.

**Interpreting Faults, Limits and Advisories**
HOTSHOT communicates important information about the way it's operating. This section explains these conditions and what you can do (if any) to respond to them.

**Fault**: Heating interrupted, fault detected, connection required
- Low Coil Flow — inadequate flow to coil (FLS1, see schematics) — assure water flow thru coil, assure plug at PJC8
- Low HStat Flow — inadequate flow to heat station (FLS1, see schematics) — assure water flow thru heat station, assure plug at CJC8
- Coil aborted / Incorrect cap — level out of range, START received after 4 sec — clean coil (remove water bridging), call Ameritherm (new caps)

**Limit (auto)**: This condition arises when the unit has attained a mis-match. It lets you know that output than expected is being restricted due to the condition (Cutback Mode may be displayed)
- Cap Over-Voltage — capacitor protection — reduce Setpoint
- Cap Over-Current — heat sink over critical temp — correct adequate water flow
- Hot Sink Hot — internal air over critical temp — ensure adequate water flow, confirm interval fan works
- Adjust Tap/Cap — to improve output/setpoint match — replace setpoint / part out of coil, thin Cure? / follow adjust taps with increased setpoint

**Cutback Mode / CAP TEMP PROTECT / REDUCE OUTPUT** — you may be able to correct frequency (Adjust Caps) or fewer coil turns (increase frequency) if your Ameritherm coil design allows Ameritherm for replacement caps.

**Limit (manual) — L\***: Set L-asterisk on display. It enabled (§3.3), occurs if the output falls or rises outside a Lo — Hi band. Use output tab to signal your equipment.

**Advisory**: See mark on display. Indicates configuration are not well matched; back out efficient configuration may be possible through output is within acceptable margins. See §3.4 to signal your equipment.

**Multi-step profile**
Cause the part is brought to temperature and held in three steps. From Control zone.

**Do this:**
- select ▶Adjust Profiles then ▶
- press ⓹ this selects Profile A
- with A1 selected, press ▲
- use ▲/▼ set A1 Start level (ex: sets A1 Start = 100A)
- press ▶
- use ▲/▼ set A1 End level (ex: sets A1 End = 10A)
- press ▶
- use ▲/▼ set A1 step Time (ex: sets A1 Time = 10s)
- press ▶
- use ▲ select A2
- set A1 Data points as shown

**Observe:**
- Select Profile ▶A B C D
- End | Start | Time
- 0.0A | Untimed | ▶A1 Data
- 100A | Start | ▶100A | 10s | ▶A1 Data
- 100A | End | 10s | ▶A1 Data
- 50A | End | 10s | ▶A1 Data
- 100A | 20s | ▶A2 Data
- 200A | 10s | ▶A3 Data

This step activates File A. HeatOn, note three: Filed can be (de)activated any time by selecting Control From (Front Panel/Rear Panel) then select using ⓹.
- press ⓹ Home
- press START

★Ready; ★HeatOn, note three: countdown until three... Ready only.

**Heating profile graph** (3-step example): Current (Amps) axis 0-200 in steps of 100; time (seconds) axis 0-40. Heating profile "A" shows a ramp/step shape through A1, A2, A3 zones.
- Untimed profile steps are skipped
- Untimed and zero-Setpoint steps are interpreted as STOP (until STOP)
- (A4 implied, above). From the Home zone, Untimed heat cycles are continuous (until STOP)

### 2.4 Equipment Maintenance

The effectiveness and safety of your HOTSHOT equipment can be assured for years by following these simple maintenance steps:

| Frequency | Check | |
|---|---|---|
| 1 month | safety | while ★Ready, turn off water; assure ★Fault, ensure water is stuck. If not ★Fault, ▶Equipment Further Equipment safety is compromised. Call Ameritherm |
| 6 months | coolant | level, top-off if required; appearance: flush & replace if dark or cloudy; system, for signs of leaks; repair |
| 12 months | coolant equipment | dump, flush, replace coolant; wiring: assure all wiring is OK, replace if damaged; setup: confirm ▶RF-Setup, Caps, Taps contact |

**NOTE**: if you use a heat transfer fluid in your equipment, dispose of spent chemicals responsibly; call an expert.

### 2.5 Smartburst Technology

The HOTSHOT heat stations contain one or more water cooled capacitors, rated for current under laboratory (rather than real-world) conditions. The Smartburst technology, which allows users to approach these kW systems make use of Ameritherm technology under actual real-world conditions while maintaining higher current limits in actual use under many real-world limits for the levels in use, allowing the safety margins required for long operating life.

Smartburst considers both thermal (mass constants) and self-heating in the capacitors to allow higher currents out of the heat station as long as the average value is safe, and if the continuous current on time is within limits for the levels in use. Beyond higher limits on time, levels are brought to a safe value.

CAP TEMP PROTECT and REDUCE OUTPUT messages are displayed on the status page, and levels are brought to a safe value.

The following chart illustrates operation for a capacitor with a laboratory current rating of ~600A. When current becomes active, coil current is held with a ceiling of about 80% of the laboratory value (750A). When the limit becomes active, coil current is held with a ceiling of about 80% of the laboratory value (~600A).

**Continuous Heat Time vs Coil RF current** (graph): Time (s) axis 0-900+ in steps of 150/300/450/600/750; Current (A) axis 600-750. Curve shows time drops steeply as current approaches ~750A (near-vertical near 750A), and rises toward 900s+ as current drops toward 600A.

---

## 3. Customizing Your HOTSHOT

You can control your HOTSHOT in different ways:
- START/STOP can be controlled from the front panel or remotely
- output level can be set from the front panel, remotely or with optional multi-step profiles you specify
- the state of your unit can be monitored remotely
- your unit can be efficiently matched to the output with RF tap settings — unit cases when configuration when your coil design requires it

### 3.1 Rear Panel Connections

Remote operation of your HOTSHOT requires electrical connections at CTB1 and — in some cases — unit configuration from the touch-pad.

**CTB1 terminal layout (rear panel, 24 pins, numbered 1-24):**

| CTB1 pin | Label | Notes |
|---|---|---|
| 1 | Remote input − | |
| 2 | Remote input + | |
| 3 | Remote start/stop | START |
| 4 | Remote start/stop | COM |
| 5 | Remote start/stop | STOP |
| 6 | FLS | flow switch |
| 7 | FLS | flow switch |
| 8 | Ready | output |
| 9 | Ready | output |
| 10 | HeatOn | output |
| 11 | HeatOn | output |
| 12 | Fault | output |
| 13 | Fault | output |
| 14 | 24V source & E-stop | ground |
| 15 | 24V source & E-stop | E-stop |
| 16 | 24V source & E-stop | E-stop |
| 17 | Aux Input | |
| 18 | Aux Input | |
| 19 | Aux Output | |
| 20 | Aux Output | |
| 21–24 | Serial | Rx-, Rx+, Tx-, Tx+ |

(Also labeled on the panel diagram: Remote Input, Remote start/stop, FLS, Ready/HeatOn/Output/Fault Output, 24V source & E-stop, Aux Input, Aux Output, Serial.)

**Remote Input (CTB1:1-2)**
Case: use an external signal source to set the desired output level of the HOTSHOT depending on the signal source you plan to use (0-10Vdc is the default configuration), follow these instructions to change.

Electrical:
- Using 0-10Vdc source: CTB1:1 = positive, CTB1:2 = negative; set jumper **upper**
- Using 4-20mA source: CTB1:1 = positive, CTB1:2 = negative; CTB1:1→2 = 250Ω resistor (included); set jumper **lower**

Touch-pad: ▶System Options → Control From → ▶Rear Panel

Observe polarity with these connections! Never reverse polarity on your analog voltage or current source.

Result: Setpoint established and held with your analog signal source; to control STOP/START remotely, also see below.

**START/STOP (CTB1:3-5)**
Case: use two external signals, switches or isolated contacts to control RF heating: START signal is normally-closed, active-closed. STOP signal is normally-closed, active-open.

*2-Button:*
Electrical — Start signal: CTB1:3 = START, CTB1:4 = COM. Stop signal: CTB1:4 = COM, CTB1:5 = STOP. normally-closed, active-closed at CTB1:3-4. normally-closed, active-open at CTB1:4-5.
Touch-pad: ▶System Options → Start From → Rear Panel; Start Type ▶2 button; ⓹ to Control, ⓺ to Home
Result: normal START signal (momentary closure) initiates heating, opening STOP signal halts heating. Heating cannot commence whenever normally-closed STOP signal is opened.

*1-Button:*
Attach your signal: CTB1:4 = COM, CTB1:5 = STOP
Touch-pad: ▶System Options → Start From → Rear Panel; Start Type ▶1 button; ⓹ to Control, ⓺ to Home
Result: 1-button Start initiates heating, which continues while STOP signal is active. Heating cannot commence whenever isolated contact to control STOP is opened.

⚠ 1-button Start is not recommended if you are using the timer while this continues while START button is held too long. It may re-start if button is partially depressed or is held too long.

**Flow switches (CTB1:6-7)**
The flow switches protect your HOTSHOT from thermal damage of cooling water to the heat sink or to the coil if inadequate.

If your coil presents a restricted water path, Ameritherm may recommend the use of a bypass block with your heat station (§4.4). If your unit ships with your standard bypass block, an external flow switch may be required to complete wiring for an external flow switch (sold separately).

To add your optional normally-open flow switch, remove jumper at CTB1:6-7, connect your flow switch to your flow switch to either the internal flow switch(es) or an external flow switch; move FLS3 connection to switches will then result in ★Fault.

To monitor the coil with your external FLS only, add jumper connect (not shown, right):
- FLS2 not used, JMP added, connect FLS2 to CTB1 6-7, LN8
- FLS2 monitors coil, JMP added, connect FLS2 to CTB1 6-7, LN8

**Status signals (CTB1:8-13)**
Case: You connect your 24Vdc source and monitoring equipment (ex., current contacted LEDs) to remotely track the status of the HOTSHOT.

Electrical — Connect to internal solid-state contacts to complete your remote status signaling (shown):
- CTB1 8-9 closure = ★Ready
- CTB1 10-11 closure = ★HeatOn
- CTB1 12-13 closure = ★Fault

Touch-pad: None
No polarity to these connections; 1A limit.
Result: Your remote 24Vdc status signals echo HOTSHOT status LEDs.

**24V source & E-stop (CTB1:14-16)**
Case: You use internal 24V power supply to drive your remote status signaling (ex: LEDs), you can include your E-stop switch.

Electrical — Connect to internal solid-state contacts to complete your remote status signaling (shown):
- CTB1 8-9 closure = ★Ready
- CTB1 10-11 closure = ★HeatOn
- CTB1 12-13 closure = ★Fault

No polarity to these connections; 1A limit. Connect your N.C. E-stop switch across CTB1 15-16 [ground reference at 14].
Touch-pad: None
Result: Your remote 24Vdc status signals echo HOTSHOT status LEDs. Reset E-stop to restart HOTSHOT at Home zone.

⚠ If no E-stop device is used, CTB1:15-16 **must be jumpered** (factory configuration); this jumper routes all 24Vdc to the controls/equipment.

⚠ You can combine the Ready status signal and the STOP input to halt heating if the Alarm is triggered; this jumper routes: CTB1:4 = CTB1:8 and CTB1:3 = CTB1:9, etc.

**Auxiliary Input/Output (CTB1:17-18, 19-20)**
Feature not supported at this time.

**Serial Port (CTB1:21-24)**
Required equipment: PC running Windows 95 or higher/HyperTerminal; 305-0174 Ameritherm RS485kit (or equivalent)

Wiring: PC(Tx+) → HOTSHOT(Rx+); PC(Tx-) → HOTSHOT(Rx-); PC(Rx+) → HOTSHOT(Tx+); PC(Rx-) → HOTSHOT(Tx-)
- Tx- (pin), Tx+ (pin), Rx- (pin), Rx+ (pin) — labeled on CTB1 21-24

**Why RS485?**
RS485 serial communication protocol is selected for its noise immunity and its ability to address multiple units connected in parallel with long data lines, its ability to address multiple units connected in parallel while being developed at this setting.

**Purchased Separately**
If you purchased your HOTSHOT including this option, the rear of your equipment will look something like this picture (not shown). The converter cable may require your computer. Configuration may require attention.

**To Configure the HOTSHOT**
If purchased, we have followed these steps to purpose your HOTSHOT to make use of the serial communications feature; after this step, the port is activated, but you must take the next steps to enable serial Start From and Control From to configure while HOTSHOT takes place only when Home zone is displayed, while HOTSHOT ignores serial data on the serial port.

Do this (System Options menu):
- select ▶System Options then ▶
- select RS485 4 wire to enable the serial communications feature; after this step, the port is activated, but you must take the next steps to enable serial Start From and Control From
- press ▶ until
- press ▶ to select Terminal Mode
- select ▲ for addressing (default = 1); press ▶ to modify, ▶ 1-32 valid, 0 used for broadcast (cannot be selected)
- select ▲ to select baud rate; press ▶ to modify (default = 38400; must be 115200)
- select ▲ to select Start From → RS485 4 wire; press ▶ to select
- select ▲ to select Control From → RS485 4 wire; press ▶ to select
- complete wiring per above guide

**To Configure Your Computer**
The following is one way to configure the serial port of your computer; you may have other means of making the communication connection.

Do this:
- launch HyperTerminal from Programs → Accessories → Communication (or other serial communication application) or download from http://www.hilgraeve.com/hyperterm.exe
- enter a 'connection name' such as HOTSHOTCOMM then select
- select COM1
- 38400 baud (must match selection above)
- 8 data bits; parity = none; stop bits = 1
- flow control NONE
- click OK, save file and close HyperTerminal
- re-open HyperTerminal, open the saved file
- attach my (converter kit) 9-pin D-subMin connector to your lower port
- attach RJ-45 modular plug to lower port on rear of HOTSHOT

Configuration of your PC/converter must follow:
- Data device type: DCE / DTE?; Comm Mode: TxON/RxON
- SDM

**Command set**
This table gives examples of each command's use and the range of the data you can set in the address/command/argument.

`address.command.argument` — [the 'setting' if required] — from the table below

Parentheses indicate default/factory setting; default=1y, 1-32 valid; Examples do not indicate absolute value; do not insert quotes' spaces'. Each command/argument must be arranged like this (case sensitive; do not insert quotes' spaces'). Device/command/argument pairs are required. broadcast address 0 = broadcast (all units will respond (Ex: 0 not))

| Comm | Arg | Result | Example |
|---|---|---|---|
| data | none | returns two-line display | 1.data |
| units | none | returns data units display | 1.units |
| set | # | sets Setpoint to that value | 1.set.25 |
| stat | none | returns system status data | 1.stat |
| start | none | starts heating | 1.start |
| stop | none | stops heating | 1.stop |
| ...(additional rows not fully legible)... | | | |

(Note: page 39's full command table wasn't fully legible in the source photo — treat this table as partial. Refer to physical manual p.39 if a specific serial command must be verified.)

### 3.2 Optional Equipment

**Pendant Styles**
At this writing, there are two models available:
- 3-lights, Start/Stop (upper)
- 2-lights, Start/Stop, Power Control (lower)

The 3-light model gives Stop and Status indicator in your hand. The 2-light (Power Control) model lets you adjust the Setpoint from the pendant, as well as ★Ready and ★HeatOn are implied but is always available at the front panel.

**3-light model**
Do this: follow instruction under §1 to assemble your equipment
- attach pendant connector cable to rear of unit at CTB1
- adjust value at front panel; use pendant to STOP/Start/Stop operations)
Touch-pad: ▶System Options: Start From ▶Rear Panel; Control From ▶Front Panel; Start Type ▶2 button
Observe: polarity, the way; visible source screen. ★Ready → ★HeatOn

**2-light model**
Touch-pad: ▶System Options: Start From ▶Rear Panel; Control From ▶Rear Panel; Start Type ▶2 button
Observe: ★Ready → ★HeatOn

**Footswitch**
You can attach an optional footswitch to your HOTSHOT, enabling you to toggle ★HeatOn remotely. In the first example, the footswitch starts a timed cycle; in the second, the footswitch toggles the second, the footswitch starts remotely only while you depress the switch.

*Continuous:*
Touch-pad: ▶System Options → Start From ▶Rear Panel; Start Type ▶1 button
Electrical: CTB1:4→5 jumper retained; CTB1:3→4 flow switch N.O. contact

*Timer:*
Touch-pad: ▶System Options → Start From ▶Rear Panel; Start Type ▶2 button
Electrical: CTB1:4→5 jumper retained; CTB1:3→4 flow switch N.O. contact
- Adjust Timer for desired duration
- Depress footswitch to start timer

### 3.3 Alarm Limits

You can configure your HOTSHOT to trigger an alarm if the output level falls or rises outside a Lo – Hi band (or when the alarm is enabled) after a delay period you specify. When the system waits for the delay period, the system turns off the ★Ready (front panel), opens the relay signal to halt heating if desired.

Do this:
- select ▶System options then ▶
- select ▲/▼ Alarm Limits then ▶
- press ▶ for ON
- Then the display alternates between: minimum (<) and maximum (>)
- press ▶ then ▶ to adjust Minimum
- press ▶ then ▶ to adjust Maximum
- use ▲/▼ then ▶ to adjust Delay
- press ⓹ Home

To disable this feature: select ▶System options then ▶; select Alarm Limits; use ▲/▼ then ▶; select OFF then ▶OFF is displayed

⚠ You can combine the Ready status signal and the STOP input to halt heating if the Alarm is triggered; this jumper routes: CTB1:4 = CTB1:8 and CTB1:3 = CTB1:9, etc.

**Observe:**
- Alarm Limits ▶On/Off
- Minimum Power: set to 1000W (example)
- Maximum Power: set to 1500W (example)
- Delay: set to 5 seconds (example)

**Example graph** — Output (Watts) axis 750/1250/2000; time (seconds) axis 5/10/20. Alarm Limits = ON. Hi = 1500W, Lo = 1000W. In the example, the output level wanders outside the limits within the first 5 seconds, but no alarm is triggered. At the 8-second point, output exceeds the limit, triggering the alarm. At the 12-second point, output level falls again within the limit band and the alarm is cleared. ★Ready is restored and the Ready output relay is closed.

### 3.4 Tap Adjustment

Under certain heating conditions, HOTSHOT may "suggest" changing your RF transformer tap settings (§2.3, Advisories). For example:

during ★HeatOn, an asterisk (*) is shown on the display, the message may be:
- press ⓹ for Status

You can ignore this suggestion if your heating results are satisfactory.

⚠ If your unit cannot meet Setpoint and a Tap-change is indicated, you must adjust Tap (no part in the coil).

Tap adjustments are made with 3 wires (X, G) and a jumper (Z), which are attached in a 3x3 terminal block:
- X and Y deliver power from the supply
- G selects a winding tap selection
- Z connects other winding sections

Compare the position of the wires with the table to determine the position of the wires with the table to work head to work properly.

Heat stations are labeled per model; this example may differ from yours.

⚠ Whenever you are unsure about the correct tap selection for your application, follow this rule: **start high, work down.** Beginning from the highest transformer tap presents the least stress to the HOTSHOT.

⚠ Failure to change Tap or Cap settings following a change may result in false displays of component protection. This does not change capacitance.

**Example**: HS65°F AM72°F, Increase Tap; 250A/220A (before/after); 234.00s/#190kHz; tap table wires E F / H D B / J A C, model K.

**Follow these steps to change taps at the work head:**
Do this:
1. turn off AC power
2. use two Phillips screwdrivers to remove work-head cover
3. view work head from wiring side (shown)
4. determine present setting from table above
5. depending on the message, select the TAP you determined to be higher or lower
6. use a slotted screwdriver, loosen the wire(s) to be moved
7. re-install the wire(s) as required by the table for the TAP you've selected
8. ensure that all screws on wires are tight
9. replace work-head cover

Navigate to Menu Selection → Heat Station → select ▶Modify Tap → select the new tap setting → ⓹ press previous, follow instructions
- return to Home zone
- press ✓ press, follow instructions
- until heating without ★

---

## 4. Technical Information

### 4.1 Mechanical

**Power Supply**

| Feature | Value | Units |
|---|---|---|
| Dimensions W×D×H | 17×21×4.25 (432×533×108) | in (mm) |
| Internal cook access swept | 10 (4.5) | lb (kg) |
| Weight | 33 (15.0) | lb (kg) |
| Construction | aluminum | |
| Finish | from panel black anodized | |

**Remote Heat Station**

| Feature | Value | Units |
|---|---|---|
| Dimensions | 8 in dia × 11 (203×279) | in (mm) |
| Weight (no reel) | 10 (4.5) | lb (kg) |
| Mounting | (per panel bracket, screws) | |
| Cable | 12 (3.7) w/ cable (10 ft) | ft (m) |
| Bend radius | (includes mounting bracket, bolt 2" bar handle) | |

⚠ Your system uses and produces high voltages! Only trained or guided service personnel are authorized inside the equipment. Always turn off the unit and remove the AC line. Always turn off internal service card before attempting any internal service.

### 4.2 Electrical

*Transcribed from a sharp, straight-on photo: [[media/wiring/hotshot-manual-p46-electrical-table.jpg]] (2026-09-05) — confirms the earlier blurry attempt at this page.*

**Equipment Input**

| Description | 3.5kW | 5kW | 7.5kW | Units |
|---|---|---|---|---|
| Model | 3.5 | 5 | 7.5 | kW |
| AC voltage | 220 | 220 | 220 | Vrms ±10%, 3φ |
| Current, max | 15 | 25 | 30 | Arms |
| Frequency | 50-60 | 50-60 | 50-60 | Hz |
| Internal breaker | 20 | 30 | 30 | Arms |
| Real Power (Pin) | 4.2 | 6 | 9 | kW max |
| Apparent Power | 4.7 | 6.7 | 10 | kVA max |
| Power Factor | .95 | .95 | .95 | — |

**Customer Input**

| Port | Active | CTB1 | Description | Limits |
|---|---|---|---|---|
| Remote input | analog | 1-2 | 0-10Vdc (Zin=21kΩ) or 4-20mA (Zin=250Ω) | see §3 |
| Start | closed | 3-4 | provide isolated contact closure* | |
| Stop | open | 4-5 | provide isolated contact closure* | |
| flow switch | closed | 6-7 | opened contact = ★Fault | 19Vdc @ 0.1A |
| E-stop | open | 15-16 | opened contact = stop | 3Adc min |

*Contacts rated for 24Vdc min, required wetting current 3mA.

**Equipment Output**

| Description | Value | Units |
|---|---|---|
| Frequency | 150-400 | kHz |
| RF Current¹ | regulated | Apk max |
| P load deliverable² | 3.5, 5, 7.5 | kW max |
| Eff'y, line-coil | >90 | % |

1) Unit limits max based on tap setting, heat station model and frequency; §3.4 for transformer settings information.
2) Using 2", 4-turn coil with matched calorimeter.

**Customer Output**

| Port | Active | CTB1 | Description | Limits |
|---|---|---|---|---|
| Ready | closed | 8-9 | isolated solid-state output, non-polarized | 24Vdc @ 1A |
| Heat On | closed | 10-11 | isolated solid-state output, non-polarized | 24Vdc @ 1A |
| Fault | closed | 12-13 | isolated solid-state output, non-polarized | 24Vdc @ 1A |
| 24V dc | | 14-16 | ground referenced | |
| Aux Port | | 17-20 | not supported at this time | 24V ±2% @ 1A |
| Serial Port | | 21-24 | | |

### 4.3 Environmental

| | Range | Units |
|---|---|---|
| ambient temp | 40 - 95 (4 - 35) | °F (°C) |
| water temp | 68 - 95 (20 - 35) | °F (°C) |
| flow (system minimum) | 1.5 - 5.7 | gpm (l/m) |
| pressure | 40 - 80 (2.8 - 5.5) | psi (bar) |
| pH | (unclear) | pH |
| conductivity | less than 65000 | (units unclear from photo) |
| dew point margin | greater than 5°F (2.5°C) below ambient¹ | °F (°C) |

1) Water temperature must not fall below the dew point in any case; condensation may result in a moisture problem or damage to the equipment.
2) You must ensure that the differential (inlet minus outlet) pressure falls within this range.

---

## Gaps / not transcribed

- **§5 Theory of Coil Design (p.51+)** — not photographed; not in this transcription.
- **Command set table (p.39)** — partially illegible in the source photo; only a handful of example rows transcribed. Re-photograph if a specific serial command needs verification.
- Several early Assembly/Cooling paragraphs (§1.4) had OCR-garbled wording in the source photos (blurry/rotated); transcribed as faithfully as possible but treat oddly-phrased sentences there as approximate, not verbatim.
- Any values marked "(unclear from photo)" above should be re-verified against the physical manual or a sharper photo before being treated as authoritative.

## See also
- [[Design/Wiring/Electrical System.md]] — curated/verified subset of this data (CTB1 pinout table, power ratings) integrated into the project's wiring design
- [[Design/Wiring/TODO - HOTSHOT Docs & E-Stop-Wiring.md]] — action items depending on this manual's content
