classdef lssys < handle &dynamicprops
    %UNTITLED13 Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        buffers@lsBuffer;
        sampfreq = 250000000;
        tick=256;
        func_name_init;
        func_name_process;
    end
    
    methods
        function initialise(obj)
            obj.func_name_init(obj);
        end
        function process(obj)
            obj.func_name_process(obj);
        end
        function obj = lssys(name, n, tick, fs)
           if nargin > 1
            %obj.buffers(1:n)= lsBuffer('ss', tick);
            for i=1:n
                obj.buffers(i) = lsBuffer(sprintf('ss_%03d',i), tick);
            end
            obj.sampfreq = fs;
            obj.func_name_init = str2func(sprintf('%s_init', name));
            obj.func_name_process = str2func(sprintf('%s_process', name));
            obj.tick = tick;
           end
        end
    end
    
end

