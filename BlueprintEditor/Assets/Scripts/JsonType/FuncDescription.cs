using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BPJsonType
{
    class FuncDescription
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public string functionName { get; set; } = "";
        /// <summary>
        /// 参数符号名称
        /// </summary>
        public List<string> paramsList { get; set; } = new List<string>();
        /// <summary>
        /// 参数类型
        /// </summary>
        public List<string> paramsTypes { get; set; } = new List<string>();
        /// <summary>
        /// 返回类型
        /// </summary>
        public string retType { get; set; } = "";
        /// <summary>
        /// 函数描述
        /// </summary>
        public string funcdes { get; set; } = "";
    }

    class OutFileJson
    {
        public string className { get; set; } = "";

        public FuncDescription[] funcs { get; set; } = new FuncDescription[0];


    }

    static class NodeFuncJsonClass
    {
        public static OutFileJson[] allClassInfo { get; set; }

        public static Dictionary<string,FuncDescription> allFunction = new Dictionary<string,FuncDescription>();
        public static Dictionary<string, List<FuncDescription>> allRetFunction_order = new Dictionary<string, List<FuncDescription>>();
  


        public static void init()
        {
            allClassInfo = JsonHelp.JsonHelper.ReadTreeInfo<OutFileJson[]>();
            if(allClassInfo == null)
            {
                return;
            }
            for (int i = 0; i < allClassInfo.Length; i++)
            {
                var info = allClassInfo[i];
                
                for (int j = 0; j < info.funcs.Length; j++)
                {
                    var func = info.funcs[j];
                    func.paramsList.RemoveAt(0);
                    func.paramsTypes.RemoveAt(0);
                    allFunction.Add(func.functionName, func);
                    string retty = func.retType;
                    List<FuncDescription> list;
                    if (!allRetFunction_order.TryGetValue(retty, out list))
                    {
                        list = new List<FuncDescription>();
                        allRetFunction_order[retty] = list;

                    }
                    list.Add(func);

                }
            }
        }
    }


}
