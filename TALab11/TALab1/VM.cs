using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TALab1
{
    class VM : INotifyPropertyChanged
    {
        Model _model = new Model();
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string inputText;
        public string InputText
        {
            get 
            { 
                return inputText;
            }
            set
            {
                inputText = value;
                OnPropertyChanged("Output");
            }
        }
        public string Output => _model.Rezult(InputText);
    }
}   
