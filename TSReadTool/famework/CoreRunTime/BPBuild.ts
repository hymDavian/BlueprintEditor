import { FunctionLibrary } from "./ActionLibrary";
import { BlueprintLogic } from "./BPObject";
import { NodeBase, TickNode, ActionNode, BreakNode, ConditionNode, ContinueNode, GetBPVarValue, GetConstValue, GetFunctionValue, VarChangeLisNode, LoopNode, WhileNode } from "./NodeLibrary";

export enum enodetype {
    ticknode, lisnode, whilenode, loopnode, getbpvnode, getcvnode, getfvnode, conditionnode, actionnode, breaknode, continuenode
}
export interface INodejson {
    position: { x: number, y: number }
    guid: string,
    type: enodetype
}
export type Ticknodejson = INodejson & {
    opennodes: string[],
    closenodes: string[],
    ticknodes: string[],
    init: number,
    csstate: number//0客户端1服务器2双端
}
export type Lisnodejson = INodejson & {
    runnodes: string[],
    liskey: string,
    init: number,
    csstate: number//0客户端1服务器2双端
}
export type Whilenodejson = INodejson & {
    conditionnode: string,
    runnodes: string[]
}
export type Loopnodejson = INodejson & {
    loop: number,
    runnodes: string[]
}
export type Getbpvnodejson = INodejson & {
    getkey: string
}
export type Getcvnodejson = INodejson & {
    v: string,
    t: evartypes
}
export type Getfvnodejson = INodejson & {
    f: FuncDescription,
    fpsnodes: string[]
}
export type Conditionnodejson = Getfvnodejson & {
    truenodes: string[],
    falsenodes: string[]
}
export type Actionnodejson = Getfvnodejson;
export enum evartypes {
    stringValue, numberValue, booleanValue
}
//构建一个蓝图对象的信息
export type BPjson = {
    allnodes: INodejson[],
    ticks: string[],
    liss: string[],
    vars: { k: string, v: string, t: evartypes }[]
}

export type FuncDescription = {
    class?: string,
    functionName: string,//函数名
    paramsList: string[],//参数列表名
    paramsTypes: string[], //参数列表需求类型组 0-string 1-number 2-bool
    retType: string, //返回类型
    funcdes: string //函数描述
}
export namespace SkillFuncLib {
    export const Actions: Map<string, (...arg: any[]) => void> = new Map();
    export const Conditions: Map<string, (...arg: any[]) => boolean> = new Map();
    export const GetValues: Map<string, (...arg: any[]) => any> = new Map();
    export let isClient: () => boolean;
    export let isServer: () => boolean;

    export function init(funcs: FuncDescription[]) {
        let a = new FunctionLibrary();
        funcs.forEach(val => {
            if (val.class === "FunctionLibrary") {
                if (val.retType === "void") {
                    Actions.set(val.functionName, a[val.functionName]);
                }
                else {
                    GetValues.set(val.functionName, a[val.functionName]);
                    if (val.retType === "boolean") {
                        Conditions.set(val.functionName, a[val.functionName]);
                    }
                }
            }
        })
    }
}



//构建一个蓝图运行对象
export function buildBpLogic(id: number, json: BPjson) {
    const allnodes: Map<string, [NodeBase, INodejson]> = new Map();
    const s = new BlueprintLogic(id);
    //先预生成所有节点对象
    for (const nodejson of json.allnodes) {
        let node: NodeBase = null;
        switch (nodejson.type) {
            case enodetype.ticknode: node = new TickNode(nodejson["csstate"], s); break;
            case enodetype.lisnode: node = new VarChangeLisNode(nodejson["csstate"], s); break;
            case enodetype.actionnode: node = new ActionNode(s); break;
            case enodetype.breaknode: node = new BreakNode(s); break;
            case enodetype.conditionnode: node = new ConditionNode(s); break;
            case enodetype.continuenode: node = new ContinueNode(s); break;
            case enodetype.getbpvnode: node = new GetBPVarValue(s); break;
            case enodetype.getcvnode: node = new GetConstValue(s); break;
            case enodetype.getfvnode: node = new GetFunctionValue(s); break;
            case enodetype.loopnode: node = new LoopNode(s); break;
            case enodetype.whilenode: node = new WhileNode(s); break;
        }
        allnodes.set(nodejson.guid, [node, nodejson]);
    }
    //执行节点内容填充
    allnodes.forEach(([node, json]) => {
        const buildFunc = buildFuncMap.get(json.type);
        buildFunc && buildFunc(node, json, allnodes);
    });
    waitTickNodeActive.forEach(({ node, active }) => {
        node.active = active;
    })
    waitTickNodeActive.length = 0;

    json.liss.forEach(v => {
        s.addEventNode(allnodes.get(v)[0] as any);
    })
    json.ticks.forEach(v => {
        s.addTickNode(allnodes.get(v)[0] as any);
    })
    json.vars.forEach(v => {
        const value = convertStringToVal.get(v.t)(v.v);
        s.setVariable(v.k, value, false);//初始化变量不触发事件变化
    })
    //返回蓝图对象
    return s;
}
const waitTickNodeActive: { node: TickNode, active: boolean }[] = [];

type buildFunc = (node: NodeBase, json: INodejson, all: Map<string, [NodeBase, INodejson]>) => void;
const buildFuncMap: Map<enodetype, buildFunc> = new Map();
function fullnode(arr: any[], all: Map<string, [NodeBase, INodejson]>, guids: string[]) {
    for (const guid of guids) {
        const node = all.get(guid)[0];
        arr.push(node)
    }
}
//各种节点的后续构建内容填充函数
buildFuncMap.set(enodetype.actionnode, (node: ActionNode, json: Actionnodejson, all) => {
    const action = SkillFuncLib.Actions.get(json.f.functionName);//todo 根据 json.f 获取执行函数
    node.callAction = action;
    fullnode(node.funcParamsNodes, all, json.fpsnodes);
})
buildFuncMap.set(enodetype.conditionnode, (node: ConditionNode, json: Conditionnodejson, all) => {
    json.f
    const conf = SkillFuncLib.Conditions.get(json.f.functionName);//todo 条件函数
    node.getValueFunction = conf;
    fullnode(node.funcParamsNodes, all, json.fpsnodes);
    fullnode(node.falseNodes, all, json.falsenodes);
    fullnode(node.trueNodes, all, json.truenodes);
})
buildFuncMap.set(enodetype.getbpvnode, (node: GetBPVarValue, json: Getbpvnodejson, all) => {
    node.selectKey = json.getkey;
})
buildFuncMap.set(enodetype.getcvnode, (node: GetConstValue, json: Getcvnodejson, all) => {
    node.m_value = convertStringToVal.get(json.t)(json.v);
})
buildFuncMap.set(enodetype.getfvnode, (node: GetFunctionValue, json: Getfvnodejson, all) => {
    json.f
    const getvf = SkillFuncLib.GetValues.get(json.f.functionName);//todo 取值函数
    node.getValueFunction = getvf;
    fullnode(node.funcParamsNodes, all, json.fpsnodes);
})
buildFuncMap.set(enodetype.lisnode, (node: VarChangeLisNode, json: Lisnodejson, all) => {
    node.lisVar = json.liskey;
    node.active = json.init != 0;
    fullnode(node.runnodes, all, json.runnodes);
})
buildFuncMap.set(enodetype.loopnode, (node: LoopNode, json: Loopnodejson, all) => {
    node.loop = json.loop;
    fullnode(node.runNodes, all, json.runnodes);
})
buildFuncMap.set(enodetype.ticknode, (node: TickNode, json: Ticknodejson, all) => {
    fullnode(node.onDeactiveNodes, all, json.closenodes);
    fullnode(node.onActiveNodes, all, json.opennodes);
    fullnode(node.onTickNodes, all, json.ticknodes);
    waitTickNodeActive.push({ node: node, active: json.init != 0 });
})
buildFuncMap.set(enodetype.whilenode, (node: WhileNode, json: Whilenodejson, all) => {
    node.condition = all.get(json.conditionnode)[0] as any;
    fullnode(node.runNodes, all, json.runnodes);
})
const convertStringToVal: Map<evartypes, (v: string) => unknown> = new Map();
convertStringToVal.set(evartypes.stringValue, v => {
    return v;
})
convertStringToVal.set(evartypes.numberValue, v => {
    const ret = Number(v);
    return Number.isNaN(ret) ? 0 : ret;
})
convertStringToVal.set(evartypes.booleanValue, v => {
    const isfalse = v === "false" || v === "0"
    return !isfalse;
})


