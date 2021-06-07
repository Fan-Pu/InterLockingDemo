using GalaSoft.MvvmLight;
using InterlockingDemo.Classes;
using InterlockingDemo.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.ViewModel
{
    class ShowErrorViewModel:ViewModelBase
    {
        private ShowErrorView view;
        private Structure structure;
        private List<Tuple<string, string>> switch_with_errors;
        private List<Tuple<string, string>> section_with_errors;

        public ShowErrorViewModel(ShowErrorView v, Structure stru, List<Tuple<string, string>> switch_errors,
            List<Tuple<string, string>> section_errors)
        {
            view = v;
            structure = stru;
            switch_with_errors = switch_errors;
            section_with_errors = section_errors;
            InitProperties();
        }



        #region <属性>
        private List<SwitchErrorItem> switchErrorList;
        public List<SwitchErrorItem> SwitchErrorList
        {
            get { return switchErrorList; }
            set { switchErrorList = value;RaisePropertyChanged(); }
        }

        private List<SectionErrorItem> sectionErrorList;
        public List<SectionErrorItem> SectionErrorList
        {
            get { return sectionErrorList; }
            set { sectionErrorList = value; RaisePropertyChanged(); }
        }
        #endregion


        #region <方法>
        private void InitProperties()
        {
            InitSwitchList();
            InitSectionList();
        }

        private void InitSwitchList()
        {
            SwitchErrorList = new List<SwitchErrorItem>();
            foreach(var item in switch_with_errors)
            {
                string name = item.Item1;
                string generated = item.Item2;
                Route route = structure.Routes[name];
                string primal = route.SwitchString;
                SwitchErrorItem errorItem = new SwitchErrorItem(route.ID.ToString(), primal, generated);
                SwitchErrorList.Add(errorItem);
            }
        }

        private void InitSectionList()
        {
            SectionErrorList = new List<SectionErrorItem>();
            foreach (var item in section_with_errors)
            {
                string name = item.Item1;
                string generated = item.Item2;
                Route route = structure.Routes[name];
                string primal = route.SectionString;
                SectionErrorItem errorItem = new SectionErrorItem(route.ID.ToString(), primal, generated);
                SectionErrorList.Add(errorItem);
            }
        }
        #endregion
    }
}
