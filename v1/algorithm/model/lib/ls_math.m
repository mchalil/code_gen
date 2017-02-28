classdef ls_math

   
   methods (Static)
       function s = wrap(sum)
            bits = 60;
           minv = -2^bits;
           maxv = 2^bits-1;
           s = sum;
                if(sum > maxv)
                    s = minv + s-(maxv);
                end
                if(sum < minv)
                    dd = abs(s)-abs(minv);
                    s = maxv - dd;
                end 
       end
      function s = add( a, b)
                s = ls_math.wrap(a+b);

      end
      function s = sub( a, b)
                s = ls_math.wrap(a-b);      
      end
      function s = mul( a, b)
                s = ls_math.wrap(a*b);
      end
      
        function y = fix_x(x, bits)
            if(bits ==0)
                y=x;
            else
                y = fix(x*2^bits);
            end
        end

    end
end

