using code_gen_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ls_code_gen
{
    class Program
    {
        static void Main(string[] args)
        {
            string app_version = "0.1";
            Console.WriteLine("\n/* Sparrow Commandline Module Testing Utility, version {0} */", app_version);
            Console.WriteLine("/* (c) 2017 Logosent Semiconductors India Pvt. Ltd.        */ ");
            CodeGenMain codeGen = new CodeGenMain(args[0] , args[1]);
            Console.WriteLine("/* Sparrow Code Generation Library Used, version {0}       */", codeGen.version);
            Console.WriteLine("\nSparrow Code Genration Result ok.\n -> Algorithm API code written into {0} \n", codeGen.file_wrapper_api_c);
            Console.WriteLine(" -> MATLAB code written into {0}  \n", codeGen.file_wrapper_api_m);
        }
    }
}
