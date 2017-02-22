
#include "ls_code_gen_api.h"
#include "cic.h"

#define INITIAL_SHIFT (0)
#define ENABLE_PRUNING 

cic::cic(Integer BlockSz, Integer NN, Integer RR, Integer MM)
{
	nCount = 0;
	BlockSize = BlockSz;
	N = NN;
	M = MM;
	R = RR;
	for (IndexInt i = 0; i < MAX_STAGES; i++)
	{
		State[i] = 0;
	}
#ifdef ENABLE_PRUNING
	if (RR < 16)
	{   // pY[countOut++] = delayBuff[N] >> 4; // r <= 16
		pruning[0] = INITIAL_SHIFT;
		pruning[1] = 4;
	}
	else if (RR < 20)
	{ // pY[countOut++] = delayBuff[N] >> 6;  // r = 20
		pruning[0] = INITIAL_SHIFT;
		pruning[1] = 6;
	}
	else if (RR < 30) 
	{ // pY[countOut++] = delayBuff[N] >> 10;  // r = 30
		pruning[0] = INITIAL_SHIFT;
		pruning[1] = 10;

	}
	else if (RR < 40)
	{ // pY[countOut++] = delayBuff[N] >> 14;  // r = 40
		pruning[0] = INITIAL_SHIFT;
		pruning[1] = 14;
	}
	else
	{ //pY[countOut++] = delayBuff[N] >> 18;  // r = 64
		pruning[0] = INITIAL_SHIFT;
		pruning[1] = 18;
	}
#endif // ENABLE_PRUNING
}

cic::~cic()
{
}

IndexInt cic::process(tFract32 *pX, tFract32 *pY)
{
	IndexInt countOut = 0;
	for (IndexInt i = 0; i < BlockSize; i++)
	{
		State[1] += pX[i]; //  >> pruning[0];
#if 1
		for (IndexInt j = 1; j < N; j++) 
		{
			State[j + 1] += State[j];
		}
#else
		// split for flexible pruning... 
		State[2] += State[1]/2;
		State[3] += State[2];
		State[4] += State[3];
		State[5] += State[4];
		State[6] += State[5];
#endif

		nCount++;

		if (nCount%R == 0)
		{
			delayBuff[1] = State[N - 1]; // inp to the first comb

			for (IndexInt j = 1; j <= N; j++) {	// using  matlab indexing. j = 0 is unused
				delayBuff[j + 1] = delayBuff[j] - compOut[j];
				compOut[j] = delayBuff[j];
			}
#if 0
			// pY[countOut++] = delayBuff[N] >> 4; // r <= 16
			// pY[countOut++] = delayBuff[N] >> 6;  // r = 20
			// pY[countOut++] = delayBuff[N] >> 10;  // r = 30
			// pY[countOut++] = delayBuff[N] >> 14;  // r = 40
			// pY[countOut++] = delayBuff[N] >> 18;  // r = 64
#endif
			pY[countOut++] = delayBuff[N] >> pruning[1];
		}
	}
	return 0;
}

