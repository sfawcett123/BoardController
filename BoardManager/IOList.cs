using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BoardManager
{
    public class NotifyPropertyChangedImpl : INotifyPropertyChanged
    {
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // interface implemetation
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class IOData : NotifyPropertyChangedImpl
    {
        private string _Value ;
        private string _Key;
        private DateTime _Changed;
        private DateTime _Read;
        public IOData(KeyValuePair<string, string> data)
        {
            _Value = data.Value;
            _Key= data.Key;

            PropertyChanged += Input_PropertyChanged;
        }
        private void Input_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _Changed = DateTime.Now;
        }

        public string Key {
            get
            {
                return this._Key;
            }
            set
            {
                if (_Key != value)
                {
                    _Key = value;
                    NotifyPropertyChanged();
                }

            }
        }
        public string Value {
            get
            {
                return this._Value;
            }
            set
            {
                if( _Value != value ) {
                    _Value = value;
                    NotifyPropertyChanged();
                }

            }
        }

        internal bool Changed()
        {
            if (_Read < _Changed)
            {
                _Read = DateTime.Now;   
                return true;
            }

            return false;
        }
    }

    public class IOList : ObservableCollection<IOData>
    {
        public IOList()
        {

        }

        public IOList(Dictionary<string,string> dict )
        {
            foreach( KeyValuePair<string,string> kvp in dict )
            {
                AddUpdate(kvp);
            }
        }

        public IOList(KeyValuePair<string, string> kvp)
        {
            AddUpdate(kvp);
        }

        public IOList(List<string> list)
        {
            foreach( string s in list )
            {
                AddUpdate(new(s, ""));
            }
        }

        public string Serialize()
        {
            Dictionary<string, string> temp = new();

            foreach( IOData iodata in this )
            {
                temp.TryAdd(iodata.Key, iodata.Value);    
            }

            return temp.Serialize();

        }
        
        public void AddUpdate( KeyValuePair<string, string> data)
        {
            bool found = false;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Key == data.Key)
                {
                    this[i].Value = data.Value;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                this.Add(data);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> ChangedData()
        {
            var result = this.Where(x => x.Changed());
            foreach (IOData iod in result)
            {
                yield return new KeyValuePair<string, string>(iod.Key, iod.Value);
            }

        }

        internal IEnumerable<KeyValuePair<string, string>> ToKeyValuePair()
        {
            foreach( IOData iod in this)
            {
                yield return new KeyValuePair<string, string>( iod.Key, iod.Value );
            }
        }

        private void Add(KeyValuePair<string, string> data)
        {
            IOData n= new( data );
            this.Add(n);
        }
    }
}
