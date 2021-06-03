using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InterlockingDemo.Helpler;

namespace InterlockingDemo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region <properties>

        #endregion


        public MainViewModel()
        {
            InitProperties();
            InitCommands();
        }


        #region <commands>
        public RelayCommand SelectInterlockingFileCM { get; set; }
        #endregion


        #region <functions>
        public void InitProperties()
        {

        }

        public void InitCommands()
        {
            SelectInterlockingFileCM = new RelayCommand(SelectInterlockingFileFunc);
        }

        public void SelectInterlockingFileFunc()
        {
            EXCELHelper.ReadEXCELFile();
        }
        #endregion
    }
}