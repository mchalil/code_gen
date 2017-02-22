#include "ls_code_gen_api.h"
#include "fir.h"
#include "ls_math.h"
#include <stdio.h>

#if false
int fir::coef_read(char *fileName, int nSize, int *pC)
{
	int i = 0;
	FILE *fp = fopen(fileName, "r");
	for (i = 0; i < nSize; i++)
	{
		char ss[100];
		char ss1[10];
		int ret = fscanf(fp, "%s%s%d", ss, ss1, &pC[i]);
		if (ret != 3) 
			break;
	}
	nOrder = i-1;
	fclose(fp);

	return i;
}
#endif

#define TST_FIR

#define FIR_MULT(a, b) ((a)*(b))


fir::fir(Integer Order, tFract32* pCoef1)
{
	nOrder = Order;
	pState = new tSamples[TICK_SZ + nOrder - 1];

	/* memset(pState, 0, sizeof(pState)); */
	/* explicitly clear memory */
	for (IndexInt i = 0; i < TICK_SZ + nOrder - 1; i++)
	{
		pState[i] = 0;
	}
	pCoef = pCoef1;
}

Integer fir::StateAddSamples(tSamples* pValues, Integer nCount, Integer nOffset)
{
	IndexInt i;

	for (i = 0; i < nCount; i++)
	{
		pState[nOffset + i] = pValues[i];
	}
	return 0;
}

Integer fir::StateShiftSamplesLeft(Integer nCount, Integer nOffset)
{
	IndexInt i;

	for (i = 0; i < nCount; i++)
	{
		pState[i] = pState[nOffset + i];
	}
	return 0;
}

Integer fir::process(tSamples *pX, tSamples* pY, Integer nCount)
{
	IndexInt outIdx = 0;

	/* add the new nCount samples to the end of the state buffer */
	StateAddSamples(pX, nCount, nOrder - 1);

	for (IndexInt n = 0; n < nCount; n++) {
		tFract32 *pCoefTemp = pCoef;
		tSamples *pSamples =  &pState[nOrder - 1 + n];
		tSamples acc = 0;

		for (IndexInt k = 0; k < nOrder; k++) {
			acc += FIR_MULT((*pCoefTemp++) , (*pSamples--));
		}
		pY[n] = acc;
	}

	/* move nOrder-1 state samples from end to beginnig to new samples can be added
	during the next call */
	StateShiftSamplesLeft(nOrder - 1, nCount);

	return 0;
}

fir::~fir()
{
	delete(pState);
}
#ifdef TST_FIR
Integer fir::tst()
{
	Integer ret = 0;
	tFract32 aFIRCoef[] = {1, 2, 3, 2, 1};
	tSamples aSamples[] = {1,2, 3, 4, 5, 1, 1, 2, 1,2, 3, 4, 5, 2, 1 , 4, 5, 1,2, 3, 4, 5, 1, 3 , 1,2, 3, 2, 5, 1, 1 };
	tSamples aSamplesOut[24];

	fir *pfir = new fir(5, aFIRCoef);
	int n = 6;
	int idx = 0;
	int frms = 4;
	Integer expectedChecSum = 536; // c = conv(a,b); s= sum(c(1:4*6)) 
	Integer checkSum = 0;

	for (int i = 0; i < frms; i++) {
		pfir->process(&aSamples[idx], &aSamplesOut[idx], 6);
		idx += n;
	}
	for (idx = 0; idx < frms*n; idx++)
	{
		checkSum += aSamplesOut[idx];
	}
	if (checkSum != expectedChecSum)
		return checkSum - expectedChecSum;

	return 0;
}
int main()
{
	int result = fir::tst();
	if (result != 0)
	{
		printf("fir test failed\n");
	}
	return result;
}
#endif
