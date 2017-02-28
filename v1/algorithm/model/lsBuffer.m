classdef lsBuffer < handle
    %UNTITLED12 Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        data;
        stride;
        id;
    end
    
    methods
        function obj = lsBuffer(id, size)
            if nargin > 0
                obj.data = zeros(1,size);
                obj.stride = 1;
                obj.id = id;
            end
        end
    end
    
end

