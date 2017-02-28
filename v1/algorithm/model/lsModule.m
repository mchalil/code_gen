classdef lsModule <   dynamicprops & handle
    %UNTITLED15 Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        nameString
    end
    properties (Constant)
        tickSz = 96;
    end
    methods
        function  obj = lsModule(name, lss)
            obj.nameString = name;
            lss.addprop(name);
            lss.(name) = obj;
            obj.addprop('lssys');
            obj.('lssys') = lss;
        end
        function obj = addp(obj, name, value)
            obj.addprop(name);
            obj.(name) = value;
        end
        
        function plot(varargin)
         if nargin == 1
            obj = varargin{1};
            idx=2;
            if(numel(obj.pII) > 0)
                legend_txt = cell(1);
                legend_txt{1} = strrep(sprintf('%s_%d', obj.nameString, obj.pII{1}{1}),'_','\_');
                subplot(2,1,1);
                plot(obj.lssys.buffers( obj.pII{1}{1}).data); hold;
                for i=2:numel(obj.pII)
                    plot(obj.lssys.buffers( obj.pII{i}{1}).data); 
                    legend_txt{idx} = strrep(sprintf('%s_%d', obj.nameString, obj.pII{i}{1}),'_','\_');
                end
                hold;
                legend(legend_txt);                
                title(sprintf('input to %s', obj.nameString), 'interpreter', 'none');
            end
            idx=2;
            if(numel(obj.pOO) > 0)
                legend_txt = cell(1);
                legend_txt{1} = strrep(sprintf('%s_%d', obj.nameString, obj.pOO{1}{1}),'_','\_'); 
                subplot(2,1,2);
                    plot(obj.lssys.buffers( obj.pOO{1}{1}).data); hold;
                for i=2:numel(obj.pOO)
                    plot(obj.lssys.buffers( obj.pOO{i}{1}).data); hold;
                    legend_txt{idx} = strrep(sprintf('%s_%d', obj.nameString, obj.pOO{i}{1}),'_','\_'); 
                end
                hold;
                legend(legend_txt);                
                title(sprintf('output from %s', obj.nameString), 'interpreter', 'none');
            end

         elseif nargin == 3
            obj = varargin{1};
            sch = varargin{2};
            file = varargin{3};
            
            idx=2;
            if(numel(obj.pOO) > 0)
                legend_txt = cell(1);
                legend_txt{1} = strrep(sprintf('%s_%d (%d)', obj.nameString, obj.pOO{1}{1},1), '_','\_'); 
                
                stride = obj.lssys.buffers( obj.pOO{1}{1}).stride;
                    subplot(2,1,1);plot(obj.lssys.buffers( obj.pOO{1}{1}).data(1:stride:end)); hold;
                    filename = sprintf('file_out_%s_%s_%d.txt', sch, file, obj.pOO{1}{1});
                    y2 = load(filename);
                    subplot(2,1,2);
   %                 y3 = y2(1:96:end);
                    plot(y2);hold;
                for i=2:numel(obj.pOO)
                    subplot(2,1,1);plot(obj.lssys.buffers( obj.pOO{i}{1}).data); 
                    filename = sprintf('file_out_%s_%s_%d.txt', sch, file, obj.pOO{i}{1});
                    subplot(2,1,2);plot(load(filename));
                    legend_txt{idx} = strrep(sprintf('%s_%d (%d)', obj.nameString, obj.pOO{i}{1},i), '_','\_'); 
                end
                subplot(2,1,1);hold;
                title(sprintf('matlab output from %s', obj.nameString), 'interpreter', 'none');
                subplot(2,1,2);hold;
                title(sprintf('c code output from %s', obj.nameString), 'interpreter', 'none');
                legend(legend_txt);                
            end
         end
         subplot(2,1,1);
         xlabel('Sample Counts')
         ylabel('Buffer Value')
         subplot(2,1,2);
         xlabel('Sample Counts')
         ylabel('Buffer Value')

      end
        
    end
    
end

