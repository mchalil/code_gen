#pragma once
#define MAX_STAGES (8)
#define MAX_PRUNING_STAGES (4)

class cic
{
	tFract64 State[MAX_STAGES+1];
	tFract64 compOut[MAX_STAGES+1];
	tFract64 delayBuff[MAX_STAGES+1];

	Integer nCount;
	Integer BlockSize;
	Integer N, R, M;
	Integer pruning[MAX_PRUNING_STAGES];
public:
	cic(Integer BlockSz, Integer NN, Integer RR, Integer MM);
	~cic();
	Integer process(tSamples *pX, tSamples *pY);
	static Integer tst();
};

