namespace StaffManager.ViewModel.ServiceContracts
{
    /// <summary>
    /// A service to handle navigation between Views
    /// </summary>
    public interface IStaffManagerNavigationService
    {
        /// <summary>
        /// Edit a model (smModelBase) instance in a modal dialog.
        /// </summary>
        /// <remarks>
        /// Use dynamic here to force generic parameters to use concrete type rather than the general base class.
        /// </remarks>
        /// <param name="model">The model to be edited</param>
        /// <returns>True for save / False for cancel</returns>
        bool EditModel(dynamic model);
    }
}
