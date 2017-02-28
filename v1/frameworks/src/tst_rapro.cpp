//#include "stdafx.h"

#include "../rapro/sch/hw_sw_ddc/sw/hw_sw_ddc_software_ddc_api.h"
#include <stdio.h>

#include "ls_sparrow_algo.h"

extern genfiraxi_instance genfiraxi_ps_pfir;
extern genfiraxi_instance genfiraxi_ps_cfir;

extern tParamFract mycoeff_pfir[48];
extern tParamFract mycoeff_cfir[24];


void main()
{
	int test1 = 12;

	FILE *fp_out;
	FILE *fp_out1;
	tSamples *pOutput0 = GETOUT_PTR_0;
	tSamples *pOutput1 = GETOUT_PTR_1;

	printf("testing\n");
	fopen_s(&fp_out, outfileName0, "w");
	fopen_s(&fp_out1, outfileName1, "w");
	int i = 0;
	
	//genfiraxi_ps_pfir.pFIRCoeff = mycoeff_pfir;
	//genfiraxi_ps_cfir.pFIRCoeff = mycoeff_cfir;
	
	//INIT_CALL(ddc, hw_sw_ddc_hardware);
	INIT_CALL(ddc, hw_sw_ddc_software);

	for (int frm = 0; frm < 800; frm++)
	{
		PROCESS_CALL(ddc, hw_sw_ddc_software);
#if 0
		for (i = 0; i < TICK_SZ; i+= GETOUT_STRIDE0)
		{
			fprintf(fp_out, "%f\n", pOutput0[i]);
		}
#endif

		for (i = 0; i < TICK_SZ; i += GETOUT_STRIDE1)
		{
			fprintf(fp_out1, "%f\n", pOutput1[i]);
		}
	}

	lss_ddc_hw_sw_ddc_software_close(0);

	fclose(fp_out1);
	fclose(fp_out);
	printf("ok\n");
	printf("output save to %s\n", outfileName0);
	printf("output save to %s\n", outfileName1);
}