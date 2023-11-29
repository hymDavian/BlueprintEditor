
/**服务器逻辑末尾帧同步下来的逻辑对象驱动变化消息包 */
class StatescriptDeltas {
    /**有新对象注册了逻辑树实例[实体id_树资源id_stateid,实体id_树资源id_stateid,...] */
    public registerLogicTree: string[];

    /**逻辑对象自身的局部变量修改,[stateid_name-val_name-val , stateid_name-val_name-val , ...    ] */
    public setLocVars: string[];


    /**此次操作移除的逻辑树对象 */
    public removeLogicTree: number[];

}

