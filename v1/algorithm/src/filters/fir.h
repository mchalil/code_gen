#pragma once
#define MAX_FIR_ORDER (512) // todo

class fir
{
	tSamples *pState;
	tFract32 *pCoef;
public:
	Integer nOrder;
	fir(Integer Order, tFract32* pCoef);
	Integer process(tSamples *pX, tSamples* pCoef, Integer nCount, Integer stride);
	Integer StateAddSamples(tSamples* pY, Integer nCount, Integer Offset, Integer stride);
	Integer StateShiftSamplesLeft(Integer nCount, Integer Offset);
	static Integer tst();
	~fir();
};

