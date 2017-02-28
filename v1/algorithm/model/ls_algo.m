classdef ls_algo
    %UNTITLED11 Summary of this class goes here
    %   Detailed explanation goes here
    
    properties (Constant)
    end
    
    methods (Static)
        function nco_bram_iq(h)
            out_s = h.lssys.buffers( h.pOO{1}{1});
            out_c = h.lssys.buffers( h.pOO{2}{1});
            phase_incr = 2*pi*h.frequency/h.lssys.sampfreq;
            t = [h.phstate:phase_incr:h.phstate + phase_incr*(h.lssys.tick-1)];

            out_s.data = h.amplitude * sin(t); 
            out_s.stride = h.pOO{1}{2}; % here take from allocation. as there is no src pin 
            out_c.data = h.amplitude * cos(t); 
            out_c.stride = h.pOO{2}{2};
            h.phstate = phase_incr*h.lssys.tick;            
        end
        function nco_bram(h)
            out = h.lssys.buffers( h.pOO{1}{1});
            phase_incr = 2*pi*h.frequency/h.lssys.sampfreq;
            t = [h.phstate:phase_incr:h.phstate + phase_incr*(h.lssys.tick-1)];

            out.data = h.amplitude * sin(t); 
            out.stride = h.pOO{1}{2};
            h.phstate = phase_incr*h.lssys.tick;
        end
        function mixmod(h)
            in_1 = h.lssys.buffers( h.pII{1}{1});
            in_2 = h.lssys.buffers( h.pII{2}{1});
            out1 = h.lssys.buffers( h.pOO{1}{1});

            out1.data(1:in_1.stride:end) = in_1.data(1:in_1.stride:end).*in_2.data(1:in_1.stride:end);
            out1.stride = in_1.stride;
        end
        function root_sumsquares(h)
            in_1 = h.lssys.buffers( h.pII{1}{1});
            in_2 = h.lssys.buffers( h.pII{2}{1});
            out1 = h.lssys.buffers( h.pOO{1}{1});
            idx = 1:in_1.stride:length(in_1.data); % needs to check if this to be same as second channel
            out1.data(idx) = sqrt(in_1.data(idx).*in_1.data(idx) + in_2.data(idx).*in_2.data(idx));
            out1.stride = in_1.stride;
        end
        function scaler(h)
            in_1 = h.lssys.buffers( h.pII{1}{1});
            out1 = h.lssys.buffers( h.pOO{1}{1});

            idx = 1:in_1.stride:length(in_1.data);
            out1.data(idx) = in_1.data(idx).*h.gain;
            out1.stride = in_1.stride;
       end
        function root_sumsquares2(h)
            in_1 = h.lssys.buffers( h.pII{1}{1});
            in_2 = h.lssys.buffers( h.pII{2}{1});
            out1 = h.lssys.buffers( h.pOO{1}{1});
            out2 = h.lssys.buffers( h.pOO{2}{1});

%             p = find(in_1.data > 0);
%             n = find(in_1.data < 0);
%             out1.data(p) = sqrt((in_1.data(p).^2 + in_2.data(p).^2));
%             out1.data(n) = sqrt((in_1.data(n).^2 + in_2.data(n).^2));
%             
%             p = find(in_2.data > 0);
%             n = find(in_2.data < 0);
%             out2.data(p) = sqrt((in_1.data(p).^2 + in_2.data(p).^2));
%             out2.data(n) = sqrt((in_1.data(n).^2 + in_2.data(n).^2));
%           
            idx = 1:in_1.stride:length(in_1.data);
            out1.data(idx) = (in_1.data(idx).^2 + in_2.data(idx).^2);
            out1.stride = in_1.stride;
            out2.data(idx) = (in_1.data(idx).^2 + in_2.data(idx).^2);
            out2.stride = in_1.stride; % both same 
        end

        function softfi(h)
             out1 = h.lssys.buffers( h.pOO{1}{1}); 
             out1.data = sin(2*pi()*[1:h.lssys.tick]*13.51e6/h.lssys.sampfreq);
             out1.stride = out1.stride;
             filename = sprintf('file_in_pin_%d.txt', h.pOO{1}{1});

             fp = fopen(filename, 'w');
             fprintf(fp, '%8.2f', floor(out1.data*100));
             fclose(fp);
%             h.pOO{1}{2} = out1.stride; % end module requires this...
        end
        
        function cicdec(h)
             in1 = h.lssys.buffers( h.pII{1}{1}); 
             out1 = h.lssys.buffers( h.pOO{1}{1}); 
             rate_in = in1.stride;
             rate_out = rate_in*h.nrate;
             out1.data = double(h.cic.process(in1.data(1:rate_in:end)));
             out1.stride = rate_out;
        end
        
        function decim(h)
             in1 = h.lssys.buffers( h.pII{1}{1}); 
             out1 = h.lssys.buffers( h.pOO{1}{1}); 
             rate_in = in1.stride;
             rate_out = rate_in*h.nrate;
             out1.data(1:rate_out:end) = in1.data(1:rate_out:end);
             out1.stride = rate_out;
        end
        
        function genfiraxi(h)
            if(isempty(h.aTuningParams{2}))
                h.aTuningParams{2} = zeros(1, h.ntap-1);
            end
             in1 = h.lssys.buffers( h.pII{1}{1}); 
             out1 = h.lssys.buffers( h.pOO{1}{1}); 
             out1.stride = in1.stride;
             idx = 1:in1.stride:length(in1.data);
             [ out1.data(idx), h.aTuningParams{2}] = filter(h.aTuningParams{1}/2^17, 1, in1.data(idx), h.aTuningParams{2});
        end
        
          function ls_fw_automation_in(h)
        
        Nco.amplitude = 1;
        Nco.frequency = 13.56e6;
        Nco.sampfreq = 245760000;
        Nco.phstate = 0;

        NcoLf.amplitude = 100000;
        NcoLf.frequency = 1.0e3;
        NcoLf.sampfreq = 245760000;
        NcoLf.phstate = 0;

        phase_incr = 2*pi*Nco.frequency/h.lssys.sampfreq;
        t = [Nco.phstate:phase_incr:Nco.phstate + phase_incr*(h.lssys.tick-1)];

        phase_incr_lf = 2*pi*NcoLf.frequency/h.lssys.sampfreq;
        tLf = [NcoLf.phstate:phase_incr_lf:NcoLf.phstate + phase_incr_lf*(h.lssys.tick-1)];

        out_s = h.lssys.buffers( h.pOO{1}{1}); 
        out_s.data = Nco.amplitude * sin(t).*(1+sin(tLf)); out_s.stride = 1;

        Nco.phstate = phase_incr*h.lssys.tick;
        NcoLf.phstate = phase_incr_lf*h.lssys.tick;

        end
     
    end
    
end

