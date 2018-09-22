namespace Busidex.DAL {
    public abstract class ViewModelBase {

        public bool IsAdmin { get; set; }
        public ModelErrorsBase ModelErrors { get; set; }
    }
}
