using BPJsonType;
using JsonHelp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;
using UGUI = UnityEngine.UI;

namespace NodeUICtor
{
    internal class VariableUI
    {
        private VariableList _listUI;
        private VariableCreate _createUI;
        private FileToolsUI _fileUI;
        private readonly BPUI mgr;
        public static VariableUI instance;
        public VariableUI(RectTransform list,RectTransform create,RectTransform file, BPUI mgr)
        {
            instance = this;
            this._listUI = new VariableList(list);
            this._createUI = new VariableCreate(create);
            this._fileUI = new FileToolsUI(file,mgr);

            _listUI.OnVarDelete += this.VarDelete;
            _listUI.ClickCreate += () => { _createUI.active = true; };
            _listUI.OnVarChange = VarChange;
            _createUI.OnVarAdd += this.VarAdd;
            this.mgr = mgr;
        }

        public void VarAdd(string key, evartypes ty, string val)
        {
            _listUI.Add(key, ty, val);
            mgr.AddVar(key, new BPVariable() { k = key, t = ty, v = val });
        }

        private void VarDelete(string key)
        {
            //todo 给数据移除变量
            mgr.DeleteVar(key);
        }

        private void VarChange(string key,string value)
        {
            //todo 给数据修改变量
            mgr.SetVar(key, value);
        }

        public void ClearUI()
        {
            _listUI.DeleteAll();
        }
    }

    class VariableList
    {
        private UGUI.Button _createBtn;
        private ItemUI _itemsource;
        private List<ItemUI> _items = new List<ItemUI>();
        public VariableList(RectTransform root)
        {
            this._createBtn = root.Find("Button").GetComponent<UGUI.Button>();
            _createBtn.onClick.AddListener(() =>
            {
                ClickCreate?.Invoke();
            });
            _itemsource = new ItemUI(root.Find("Scroll View/Viewport/Content/Item"));
            _itemsource.root.gameObject.SetActive(false);
        }

        string[] tyNames = new string[]{ "string", "number", "boolean" };
        public void Add(string key, evartypes ty, string val)
        {
            var item = _itemsource.Clone();
            item.key.text = key;
            item.type.text = tyNames[(int)ty];
            if(ty== evartypes.numberValue)
            {
                item.value.contentType = UGUI.InputField.ContentType.DecimalNumber;
            }

            item.value.text = val;
            item.OnValueChange = (cval) =>
            {
                this.OnVarChange?.Invoke(key, cval);
            };
            item.OnDeleteClick = () =>
            {
                this.OnVarDelete?.Invoke(key);
                this._items.Remove(item);
                item.Destory();
            };
            _items.Add(item);
        }
        public event Action<string> OnVarDelete;
        public event Action ClickCreate;
        public Action<string,string> OnVarChange;

        public void DeleteAll()
        {
            for(int i= _items.Count-1; i>=0; i--)
            {
                this.OnVarDelete?.Invoke(_items[i].key.text);
                this._items.RemoveAt(i);
                _items[i].Destory();
            }
        }

        class ItemUI
        {
            public readonly UGUI.Text key;
            public readonly UGUI.Text type;
            public readonly UGUI.InputField value;
            public Action<string> OnValueChange = null;
            public Action OnDeleteClick = null;
            public readonly Transform root;
            private readonly Transform parent;
            public ItemUI(Transform _root)
            {
                this.root = _root;
                this.parent = _root.parent;
                this.key = root.Find("Key/Text").GetComponent<UGUI.Text>();
                this.type = root.Find("Type/Text").GetComponent<UGUI.Text>();
                this.value = root.Find("Value/InputField").GetComponent<UGUI.InputField>();
                value.onValueChanged.AddListener(s =>
                {
                    if(type.text == "boolean")
                    {
                        if(value.text!="0" && value.text != "1")
                        {
                            value.text = "0";
                        }
                    }
                    this.OnValueChange?.Invoke(value.text);
                });
                root.Find("Del").GetComponent<UGUI.Button>().onClick.AddListener(() =>
                {
                    OnDeleteClick?.Invoke();
                });
            }
            public ItemUI Clone()
            {
                var croot = GameObject.Instantiate(root, parent);
                croot.SetAsLastSibling();
                croot.gameObject.SetActive(true);
                return new ItemUI(croot);
            }
            public void Destory()
            {
                GameObject.Destroy(this.root.gameObject);
            }
        }
    }
    class VariableCreate
    {
        private RectTransform _root;
        private UGUI.Button _btnOK;
        private UGUI.Button _btnNo;
        private UGUI.InputField _key;
        private UGUI.Dropdown _type;
        private UGUI.InputField _value;
        public VariableCreate(RectTransform root)
        {
            this._root = root;
            this._btnOK = root.Find("btnOK").GetComponent<UGUI.Button>();
            this._btnNo = root.Find("btnNo").GetComponent<UGUI.Button>();
            this._key = root.Find("inputKey").GetComponent<UGUI.InputField>();
            this._type = root.Find("Type").GetComponent<UGUI.Dropdown>();
            this._value = root.Find("inputValue").GetComponent<UGUI.InputField>();
            initUI();
        }
        private void initUI()
        {
            _btnOK.onClick.AddListener(() => { 
                this.OnVarAdd?.Invoke(_key.text,(evartypes)_type.value,_value.text  ); 
                this.active = false;
            });
            _btnNo.onClick.AddListener(() =>
            {
                this.active = false;
            });
            _type.ClearOptions();
            _type.AddOptions(new List<string>() { "string","number","boolean" });
            _type.onValueChanged.AddListener(n =>
            {
                this.isBool = false;
                switch ((evartypes)n)
                {
                    case evartypes.stringValue:this._value.contentType = UGUI.InputField.ContentType.Standard; break;
                    case evartypes.numberValue:
                        this._value.contentType = UGUI.InputField.ContentType.DecimalNumber; 
                        if( !double.TryParse(this._value.text,out var d))
                        {
                            this._value.text = "0";
                        }
                        break;
                    case evartypes.booleanValue:
                        this.isBool = true;
                        this.fixBool();
                        break;
                }
            });
            this._value.onValueChanged.AddListener(s =>
            {
                if (isBool)
                {
                    this.fixBool();
                }
            });
        }
        private void fixBool()
        {
            if(this._value.text!="0" && this._value.text != "1")
            {
                this._value.text = "0";
            }
        }
        private bool isBool = false;

        public event Action<string, evartypes, string> OnVarAdd;
        private void onOpen()
        {
            _key.text = "";
            _type.value = 0;
            _value.text = "";
        }

        public bool active
        {
            get { return _root.gameObject.activeSelf; }
            set { 
                _root.gameObject.SetActive(value);
                if (value)
                {
                    this.onOpen();
                }
            }
        }
    }

    class FileToolsUI
    {
        private readonly RectTransform _root;
        private readonly UGUI.Button _exportBtn;
        private readonly UGUI.Button _loadBtn;
        private readonly UGUI.Button _newBtn;
        private readonly RectTransform _content;
        private readonly UGUI.Button _itemclone;
        private readonly BPUI _mgr;
        public FileToolsUI(RectTransform root,BPUI mgr)
        {
            this._root = root;
            this._mgr = mgr;
            _exportBtn = root.Find("exportBtn").GetComponent<UGUI.Button>();
            _loadBtn = root.Find("loadBtn").GetComponent <UGUI.Button>();
            _newBtn = root.Find("newBtn").parent.GetComponent<UGUI.Button>();
            _content = root.Find("dataList/Viewport/Content").GetComponent<RectTransform>();
            _itemclone = _content.Find("item").GetComponent<UGUI.Button>();
            _itemclone.gameObject.SetActive(false);

            _exportBtn.onClick.AddListener(Export);
            _loadBtn.onClick.AddListener(Load);
        }

        private void Export()
        {
            _mgr.BuildBPJson();
        }
        private void Load()
        {
            var data = JsonHelper.ReadTreeInfo<BPjson>(new JsonNodeConverter());
            _mgr.Load(data);
        }

        class JsonNodeConverter : JsonHelper.JsonCreateConverter<INodejson>
        {
            protected override INodejson Create(Type objectType, JObject jsonObject)
            {
                int typeName = 0;
                int.TryParse(jsonObject["type"].ToString(), out typeName);
                switch ((enodetype)typeName)
                {

                    case enodetype.ticknode:return new Ticknodejson();
                    case enodetype.lisnode: return new Lisnodejson();
                    case enodetype.whilenode: return new Whilenodejson();
                    case enodetype.loopnode: return new Loopnodejson ();
                    case enodetype.getbpvnode: return new Getbpvnodejson();
                    case enodetype.getcvnode: return new Getcvnodejson();
                    case enodetype.getfvnode: return new Getfvnodejson();
                    case enodetype.conditionnode: return new Conditionnodejson();
                    case enodetype.actionnode: return new Actionnodejson();
                    case enodetype.breaknode: return new Breaknodejson();
                    case enodetype.continuenode: return new Continuenodejson();
                    default:return null;

                }
            }
        }
    }
}
