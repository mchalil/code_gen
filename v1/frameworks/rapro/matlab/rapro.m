addpath ../../../algorithm/model ../../../algorithm/model/lib ../sch/hw_sw_ddc/matlab/sw -end

% ls_cic.tst();

%! C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcvarsall.bat
%! make -C ../build
%! make -C ../code file=impro1_cmod  bin

clear all; 
lss1 = lssys('hw_sw_ddc_software_ddc', 100, 96*4*800);

lss1.initialise();

lss1.genfiraxi_ps_pfir.aTuningParams{1} = [ ...
    
%     -1	-32	-131	-258	-238	69	452	379	-341	-966	-427	1091 ...
%     1703	-56	-2621	-2328	1817	5206	1986	-6519	-9776	2304 ...
%     26797	47051	47051	26797	2304	-9776	-6519	1986	5206	...
%     1817	-2328	-2621	-56	1703	1091	-427	-966	-341	...
%     379	452	69	-238	-258	-131	-32	-1 ...
    -63,-6,70,171,290,402,461,410,197,-203,-767,-1414,-2001,-2342,-2235,-1505,-48,2139,4932,8088,11274,14113,16245,17389,17389,16245,14113,11274,8088,4932,2139,-48,-1505,-2235,-2342,-2001,-1414,-767,-203,197,410,461,402,290,171,70,-6,-63

];
lss1.genfiraxi_ps_cfir.aTuningParams{1} = [ ...
-79,-469,-584,375,2241,2443,-1680,-7839,-7818,5316,27973,45930,45930,27973,5316,-7818,-7839,-1680,2443,2241,375,-584,-469,-79

% 1758	-1443	-4855	-1	7669	8092	-2400	-15884	-17726	849	...
%     32475	56969	56969	32475	849	-17726	-15884	-2400	8092	7669 ...
%     -1	-4855	-1443	1758		...
];
lss1.process();
lss1.decim_ps_pfir_dec.plot('ddc','hw_sw_ddc_software');
%! cd ..\\build
%! tst_impro.exe 
%lss1.root_sumsquares_rtsq_ab.plot('cmod', 'impro1')
%lss1.scaler_attenuate_rf.plot('cmod', 'impro1')

rmpath ../../../algorithm/model ../../../algorithm/model/lib  ../sch/hw_sw_ddc/matlab/sw
