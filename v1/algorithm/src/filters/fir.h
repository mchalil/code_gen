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
	Integer fir::StateAddSamples(tSamples* pY, Integer nCount, Integer Offset, Integer stride);
	Integer fir::StateShiftSamplesLeft(Integer nCount, Integer Offset);
	static Integer fir::tst();
	~fir();
};

