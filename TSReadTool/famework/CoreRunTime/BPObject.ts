import { VarChangeLisNode, TickNode } from "./NodeLibrary";


//蓝图运行时对象
export class BlueprintLogic {
    /**唯一ID */
    public readonly id: number;

    private readonly _locVariable: Map<string, any> = new Map();//局部变量
    private readonly _listenVarChange: Map<string, VarChangeLisNode[]> = new Map();//监听变量更新事件节点
    private readonly _ticknodes: TickNode[] = [];//帧更新节点

    constructor(id: number) {
        this.id = id;
    }

    public addTickNode(n: TickNode) {
        const index = this._ticknodes.indexOf(n);
        if (index === -1) {
            this._ticknodes.push(n);
        }
    }

    public addEventNode(n: VarChangeLisNode) {
        const key = n.lisVar;
        if (!this._listenVarChange.has(key)) {
            this._listenVarChange.set(key, []);
        }
        const nodearr = this._listenVarChange.get(key);
        const index = nodearr.indexOf(n);
        if (index === -1) {
            nodearr.push(n);
        }
    }

    public setVariable(key: string, value: any, callLis: boolean = true) {
        const oldvalue = this._locVariable.get(key);
        if (oldvalue == value) { return; }
        this._locVariable.set(key, value);

        if (callLis && this._listenVarChange.has(key)) {
            this._listenVarChange.get(key).forEach(node => {
                node.active && node.start();
            })
        }
    }
    public getVariable(key: string) {
        return this._locVariable.get(key);
    }

    //帧节点驱动
    public tick() {
        this._ticknodes.forEach(node => {
            node.active && node.tick();
        })
    }

    //被移除时需要执行，执行帧节点的失活逻辑
    public delete() {
        this._ticknodes.forEach(node => {
            node.active = false;
        });
    }

}
