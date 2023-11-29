import { SkillFuncLib } from "./BPBuild";
import { BlueprintLogic } from "./BPObject";

interface Class<T> extends Function {
    new(...args: any[]): T;
}

enum ERunNodeFlag {
    void, break, continue
}

/**具有实际执行意义的节点，区别于取值节点 */
export interface IRunNode {
    start(): ERunNodeFlag;
}
//节点基类
export abstract class NodeBase {
    /**自身驱动脚本对象 */
    public readonly bplogic: BlueprintLogic;
    constructor(script: BlueprintLogic) {
        this.bplogic = script;
    }
}


/**取值节点,本身被调用时啥也不做，只用于自身属性取值 */
abstract class GetValueNode extends NodeBase {
    /**此节点的计算结果值 */
    public abstract get value(): unknown;
    constructor(s: BlueprintLogic) {
        super(s);
    }
}

/**取当前树对象的变量值 */
export class GetBPVarValue extends GetValueNode {
    public selectKey: string;
    constructor(script: BlueprintLogic, key?: string) {
        super(script)
        this.selectKey = key;
    }
    public get value(): any {
        return this.bplogic.getVariable(this.selectKey);
    }

}
/**取固定值 */
export class GetConstValue extends GetValueNode {
    public m_value: any;
    constructor(script: BlueprintLogic, v?: any) {
        super(script);
        this.m_value = v;
    }
    public get value(): any {
        return this.m_value;
    }
}
/**取函数值 */
export class GetFunctionValue extends GetValueNode {
    public getValueFunction: (logic: BlueprintLogic, ...args: any[]) => any = null;//取值函数
    /**参数取值节点 */
    public readonly funcParamsNodes: GetValueNode[] = [];

    constructor(script: BlueprintLogic, f?: (logic: BlueprintLogic, ...args: any[]) => any, psnodes?: GetValueNode[]) {
        super(script);
        f && (this.getValueFunction = f);
        psnodes && this.funcParamsNodes.push(...psnodes);
    }

    private getParams(): any[] {
        const ret = [];
        for (let i = 0; i < this.funcParamsNodes.length; i++) {
            const v = this.funcParamsNodes[i].value;
            ret.push(v);
        }
        return ret;
    }

    public get value(): any {
        if (this.getValueFunction) {
            const ps = this.getParams();
            return this.getValueFunction(this.bplogic, ...ps);
        }
        else {
            return null;
        }
    }
    protected getvalue(): any {
        return this.value;
    }

}
//动作节点,叶子节点，不需要返回任何值，也不驱动任何节点
export class ActionNode extends NodeBase implements IRunNode {
    /**参数取值节点 */
    public readonly funcParamsNodes: GetValueNode[] = [];
    //自定义动作函数
    public callAction: (logic: BlueprintLogic, ...ps: any[]) => void = null;
    constructor(s: BlueprintLogic, action?: (...ps: any[]) => void, psnodes?: GetValueNode[]) {
        super(s);
        action && (this.callAction = action);
        psnodes && this.funcParamsNodes.push(...psnodes);
    }
    start(): ERunNodeFlag {
        if (this.callAction != null) {
            const ps = this.getParams();
            this.callAction(this.bplogic, ...ps);
        }
        return ERunNodeFlag.void;
    }
    private getParams(): any[] {
        const ret = [];
        for (let i = 0; i < this.funcParamsNodes.length; i++) {
            const v = this.funcParamsNodes[i].value;
            ret.push(v);
        }
        return ret;
    }
}
//条件节点(取布尔值的函数取值节点),但区别取值节点是可以执行具体事务
export class ConditionNode extends GetFunctionValue implements IRunNode {
    constructor(script: BlueprintLogic, f?: (logic: BlueprintLogic, ...args: any[]) => boolean, psnodes?: GetValueNode[], tnodes?: IRunNode[], fnodes?: IRunNode[]) {
        super(script, f, psnodes);
        tnodes && this.trueNodes.push(...tnodes);
        fnodes && this.falseNodes.push(...fnodes);
    }

    start(): ERunNodeFlag {
        const ret = this.value;
        if (ret) {
            this.trueNodes.forEach(n => { n.start(); });
        }
        else {
            this.falseNodes.forEach(n => { n.start(); });
        }
        return ERunNodeFlag.void
    }

    /**进入true分支 */
    public readonly trueNodes: IRunNode[] = [];
    /**进入false分支 */
    public readonly falseNodes: IRunNode[] = [];
}

//流程控制节点 
abstract class ControlNode extends NodeBase {
}

//跳出节点
export class BreakNode extends NodeBase implements IRunNode {
    start(): ERunNodeFlag {
        return ERunNodeFlag.break;
    }
}
//继续节点
export class ContinueNode extends NodeBase implements IRunNode {
    start(): ERunNodeFlag {
        return ERunNodeFlag.continue;
    }
}

//固定循环节点,如果次数少于1，将最少执行一次，此节点无法死循环
export class LoopNode extends ControlNode implements IRunNode {
    public loop: number;
    public readonly runNodes: IRunNode[] = [];
    constructor(script: BlueprintLogic, loopnum: number = 1, actions?: IRunNode[]) {
        super(script);
        this.loop = Math.max(1, loopnum);
        actions && this.runNodes.push(...actions);
    }

    start(): ERunNodeFlag {
        if (this.runNodes.length <= 0) { return ERunNodeFlag.void; }
        for (let i = 0; i < this.loop; i++) {
            for (let n of this.runNodes) {
                const childFlag = n.start();
                if (childFlag == ERunNodeFlag.break) {
                    return ERunNodeFlag.void;
                }
                else if (childFlag == ERunNodeFlag.continue) {
                    break;
                }
            }
        }
        return ERunNodeFlag.void;
    }
}

//条件循环节点 如果不带条件，将会无限循环执行
export class WhileNode extends NodeBase implements IRunNode {
    public condition: ConditionNode;
    public readonly runNodes: IRunNode[] = [];
    constructor(script: BlueprintLogic, con?: ConditionNode, actions?: IRunNode[]) {
        super(script);
        this.condition = con;
        actions && this.runNodes.push(...actions);
    }
    start(): ERunNodeFlag {
        if (this.runNodes.length <= 0) { return ERunNodeFlag.void; }
        while (this.condition == null || this.condition.value) {
            for (let n of this.runNodes) {
                const childFlag = n.start();
                if (childFlag == ERunNodeFlag.break) {
                    return ERunNodeFlag.void;
                }
                else if (childFlag == ERunNodeFlag.continue) {
                    break;
                }
            }
        }
        return ERunNodeFlag.void;
    }
}

//双端检查节点
class CsNode extends NodeBase {
    //0客户端1服务器2双端
    private readonly selfnetState: number;
    constructor(csstate: number, s: BlueprintLogic) {
        super(s)
        this.selfnetState = csstate;
    }

    //自身是否在正确的网络端，用于判断是否应该运行子逻辑
    protected get netRun(): boolean {
        if (this.selfnetState >= 2) {
            return true;
        }
        if (this.selfnetState === 0 && SkillFuncLib.isClient()) {
            return true;
        }
        if (this.selfnetState === 1 && SkillFuncLib.isServer()) {
            return true;
        }
        console.error("在不正确的网络端调用！");
        return false;
    }
}

/**持续逻辑驱动节点,一般作为根节点 */
export class TickNode extends CsNode {

    private _active: boolean = false;
    public get active(): boolean {
        return this._active;
    }
    public set active(v: boolean) {
        if (this._active == v || !this.netRun) { return; }
        this._active = v;
        if (this._active) {
            this.onActiveNodes.forEach(n => { n.start(); })
        }
        else {
            this.onDeactiveNodes.forEach(n => { n.start(); })
        }
    }

    public readonly onActiveNodes: IRunNode[] = [];//打开时触发子节点
    public readonly onDeactiveNodes: IRunNode[] = [];//关闭时触发子节点
    public readonly onTickNodes: IRunNode[] = [];//帧更新子节点

    public tick() {
        if (!this.netRun) {
            return;
        }
        this.onTickNodes.forEach(n => { n.start(); });
    }


}
//监听蓝图变量更改触发节点
export class VarChangeLisNode extends CsNode {
    public readonly runnodes: IRunNode[] = [];
    public lisVar: string;
    public active: boolean = true;//是否开启监听

    constructor(csstate: number, s: BlueprintLogic, actions?: IRunNode[]) {
        super(csstate, s);
        actions && this.runnodes.push(...actions);
    }

    public start() {
        if (!this.netRun) { return; }
        if (this.runnodes.length > 0) {
            this.runnodes.forEach(n => { n.start(); })
        }
    }
}


