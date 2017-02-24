//#include "stdafx.h"

#include "hw_sw_ddc_software_ddc_api.h"
#include <stdio.h>

#include "ls_sparrow_algo.h"

extern genfiraxi_instance genfiraxi_ps_pfir;
extern genfiraxi_instance genfiraxi_ps_cfir;

extern tParamFract mycoeff_pfir[48];
extern tParamFract mycoeff_cfir[24];

void main1()
{
	int test1 = 12;

	FILE *fp_out;
	tSamples *pOutput0 = GETOUT_PTR_0;

	printf("testing\n");
	fopen_s(&fp_out, outfileName0, "w");
	int i = 0;
	
	genfiraxi_ps_pfir.pFIRCoeff = mycoeff_pfir;
	genfiraxi_ps_cfir.pFIRCoeff = mycoeff_cfir;
	
	INIT_CALL(ddc, hw_sw_ddc_software);

	for (int frm = 0; frm < 10; frm++)
	{
		PROCESS_CALL(ddc, hw_sw_ddc_software);

		for (i = 0; i < TICK_SZ; i++)
		{
			fprintf(fp_out, "%f\n", pOutput0[i]);
		}
	}
	fclose(fp_out);
	printf("ok\n");
	printf("output save to %s\n", outfileName0);
}