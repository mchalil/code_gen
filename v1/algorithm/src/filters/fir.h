#pragma once
#define MAX_FIR_ORDER (512) // todo

class fir
{
	tSamples *pState;
	tFract32 *pCoef;
public:
	Integer nOrder;
	fir(Integer Order, tFract32* pCoef);
	Integer process(tSamples *pX, tSamples* pCoef, Integer nCount);
	Integer fir::StateAddSamples(tSamples* pY, Integer nCount, Integer Offset);
	Integer fir::StateShiftSamplesLeft(Integer nCount, Integer Offset);
	Integer fir::coef_read(char *fileName, Integer nSize, tFract32 *pCoef);
	~fir();
};

