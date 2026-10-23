using System;
using System.Collections.Generic;

// Same physics as tm.cs, plus the sample itself as a 10x10mm steel bar below the ceramic,
// with only a short band (coil hot zone) held at Th. z=0 is the sample/ceramic interface.
public class TP4 {
  public double tealMm=21.65, kCer=1.265, rhoCer=2490, cCer=840, eCer=0.85;
  public double rodBottom=0.0508, kRod=16.2, rhoRod=8000, cRod=500, eRod=0.35;
  public double rContact=39.54, rJoint=0, Th=800, beta=0.0, hg=15, hair=7, hjet=500, hceil=25;
  public double sealQ=0, sealC=10, rClamp=0.8;
  public bool jet=true, clamp=false;
  public double lidT=0.00795, rodAbove=0.178, clampStart=0.1066, jetOff=0.00635;
  public double sampleLen=0.055, hotLen=0.0127, hotCenterFromCeramic=0.0275, kS=30, rhoS=7850, cS=600, eS=0.8;
  public bool wholeSampleHot=false;
  public double dt=1, tEnd=1800, recEvery=10, dz=0.0005, jetHalf=0.0065, fc=1.0, qcW=18.0, z0=0.02, dL0=0.0, dLinf=0.0, tauL=3000.0, coolantC=16.0;
}

public static class TM4 {
  const double sig=5.670374e-8, Tamb=293.15, Tw=300.0;
  public static double[] LastBal; static double LayerDT(TP4 p, double t, double Thk){ double sc=(Thk-Tamb)/780.0; return (p.dL0+(p.dLinf-p.dL0)*(1.0-Math.Exp(-t/p.tauL)))*sc; } static double PlumeDT(TP4 p, double z, double Thk){ double Th=Thk-Tamb; double Qk=p.qcW*(Th/780.0)/1000.0; double ze=Math.Max(z,0.0)+p.z0; double dT=25.0*Math.Pow(Math.Pow(Qk,0.4)/ze,5.0/3.0); dT=Math.Min(dT,0.6*Th); return p.fc*dT; }
  public static double[][] Sim(TP4 p) {
    double dz=p.dz;
    double[] zl = new double[]{0.003175,0.004175,p.tealMm/1000.0,0.010};
    double[] za = new double[]{219.2e-6,118.1e-6,313.8e-6,292.9e-6};
    double Lc=0; for(int j=0;j<4;j++) Lc+=zl[j];
    double zSealB=Lc+p.rodBottom, zLidTop=zSealB+p.lidT, zEnd=zLidTop+p.rodAbove;
    int Ns=(int)Math.Round(p.sampleLen/dz);
    int N=Ns+(int)Math.Round(zEnd/dz);
    double Arod=Math.PI/4*0.0127*0.0127, Prod=Math.PI*0.0127, As=1e-4, Ps=0.04;
    double[] Ar=new double[N], Pe=new double[N], kc=new double[N], ep=new double[N], zp=new double[N], Cn=new double[N];
    int[] ty=new int[N]; bool[] hot=new bool[N];
    for(int i=0;i<N;i++){
      double zi=-p.sampleLen+(i+0.5)*dz; zp[i]=zi;
      if(zi<0){
        Ar[i]=As; Pe[i]=Ps; kc[i]=p.kS; ep[i]=p.eS; ty[i]=0; Cn[i]=p.rhoS*p.cS*As*dz;
        double dfc=-zi; hot[i]= p.wholeSampleHot || Math.Abs(dfc-p.hotCenterFromCeramic)<=p.hotLen/2;
      } else if(zi<Lc){
        double c0=0; int seg=3;
        for(int j=0;j<4;j++){ if(zi>=c0 && zi<c0+zl[j]){seg=j;break;} c0+=zl[j]; }
        Ar[i]=za[seg]; kc[i]=p.kCer; ep[i]=p.eCer; Pe[i]=2*Math.PI*Math.Sqrt(Ar[i]/Math.PI); ty[i]=1;
        Cn[i]=p.rhoCer*p.cCer*Ar[i]*dz;
      } else {
        Ar[i]=Arod; Pe[i]=Prod; kc[i]=p.kRod; ep[i]=p.eRod;
        if(zi<zSealB) ty[i]=2; else if(zi<zLidTop) ty[i]=3; else ty[i]=4;
        Cn[i]=p.rhoRod*p.cRod*Arod*dz;
      }
    }
    int iLid0=-1; for(int i=1;i<N;i++){ if(ty[i]==3 && ty[i-1]==2){iLid0=i;break;} }
    Func<double,int> idx = z => (int)Math.Floor((z+p.sampleLen)/dz);
    int iSeal=idx(zSealB+p.lidT/2), iBelow=idx(zSealB-0.006), iTop=idx(Lc-0.001), iSEnd=Ns-1, iLidTop=idx(zLidTop+0.0003), iAbove=idx(zLidTop+0.0254), iBelowHalf=idx(zSealB-0.0127), iBottom=idx(zSealB+0.0002), iRodJ=idx(Lc+0.0003);
    Cn[iSeal]+=p.sealC;
    double jetC=zLidTop+p.jetOff, clS=zLidTop+p.clampStart, Th_k=p.Th+273.15, Ghot=1e4;
    double[] T=new double[N]; for(int i=0;i<N;i++) T[i]=Tamb;
    double[] lo=new double[N], di=new double[N], up=new double[N], rv=new double[N], cp=new double[N], dp=new double[N];
    List<double[]> outL=new List<double[]>();
    int steps=(int)Math.Round(p.tEnd/p.dt), recN=(int)Math.Max(1,Math.Round(p.recEvery/p.dt));
    for(int s=0;s<=steps;s++){
      double t=s*p.dt;
      if(s>0){
        for(int i=0;i<N;i++){
          double dd=Cn[i]/p.dt, rr=Cn[i]/p.dt*T[i];
          lo[i]=0; up[i]=0;
          if(i>0){ double rg=0.5*dz/(kc[i-1]*Ar[i-1])+0.5*dz/(kc[i]*Ar[i]); if(zp[i-1]<Lc && zp[i]>=Lc) rg+=p.rJoint; if(zp[i-1]<0 && zp[i]>=0) rg+=p.rContact; double g=1.0/rg; lo[i]=-g; dd+=g; }
          if(i<N-1){ double rg=0.5*dz/(kc[i]*Ar[i])+0.5*dz/(kc[i+1]*Ar[i+1]); if(zp[i]<Lc && zp[i+1]>=Lc) rg+=p.rJoint; if(zp[i]<0 && zp[i+1]>=0) rg+=p.rContact; double g=1.0/rg; up[i]=-g; dd+=g; }
          double sa=Pe[i]*dz;
          if(ty[i]==0){
            if(hot[i]){ dd+=Ghot; rr+=Ghot*Th_k; }
            double hr=ep[i]*sig*(T[i]*T[i]+Tw*Tw)*(T[i]+Tw);
            dd+=(p.hg+hr)*sa; rr+=(p.hg*Tamb+hr*Tw)*sa;
          } else if(ty[i]<=2){
            double Tg=Tamb+PlumeDT(p, zp[i], Th_k)+LayerDT(p,t,Th_k);
            double hr=ep[i]*sig*(T[i]*T[i]+Tw*Tw)*(T[i]+Tw);
            dd+=(p.hg+hr)*sa; rr+=(p.hg*Tg+hr*Tw)*sa;
          } else if(ty[i]==4){
            double h=p.hair; if(p.jet && Math.Abs(zp[i]-jetC)<=p.jetHalf) h=p.hjet;
            double hr=ep[i]*sig*(T[i]*T[i]+Tamb*Tamb)*(T[i]+Tamb);
            dd+=(h+hr)*sa; rr+=(h+hr)*Tamb*sa;
            if(p.clamp && zp[i]>=clS && zp[i]<clS+0.0635){ double gc=(1.0/p.rClamp)*dz/0.0635; dd+=gc; rr+=gc*(273.15+p.coolantC); }
          }
          if(i==iLid0){
            double Tgs=Tamb+PlumeDT(p, zp[i], Th_k)+LayerDT(p,t,Th_k);
            double gs=p.hceil*2.6e-4; dd+=gs; rr+=gs*Tgs;
            rr+=p.sealQ*(Th_k-Tamb)/(800.0+273.15-Tamb);
          }
          di[i]=dd; rv[i]=rr;
        }
        cp[0]=up[0]/di[0]; dp[0]=rv[0]/di[0];
        for(int i=1;i<N;i++){ double m=di[i]-lo[i]*cp[i-1]; cp[i]=up[i]/m; dp[i]=(rv[i]-lo[i]*dp[i-1])/m; }
        T[N-1]=dp[N-1]; for(int i=N-2;i>=0;i--) T[i]=dp[i]-cp[i]*T[i+1];
      }
      if(s%recN==0) outL.Add(new double[]{t, T[iSEnd]-273.15, T[iSeal]-273.15, T[iBelow]-273.15, T[iTop]-273.15, T[iLidTop]-273.15, T[iAbove]-273.15, T[iBelowHalf]-273.15, T[iBottom]-273.15, T[iRodJ]-273.15, Tamb+PlumeDT(p, zSealB, Th_k)+LayerDT(p,t,Th_k)-273.15});
    }
    // heat crossing the sample/ceramic contact at the final state (W)
    double rgc=0.5*dz/(kc[Ns-1]*Ar[Ns-1])+0.5*dz/(kc[Ns]*Ar[Ns])+p.rContact;
    LastBal=new double[]{ (T[Ns-1]-T[Ns])/rgc };
    return outL.ToArray();
  }
}






