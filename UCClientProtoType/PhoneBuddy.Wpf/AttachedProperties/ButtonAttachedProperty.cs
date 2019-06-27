namespace PhoneBuddy.Wpf
{
    /// <summary>
    /// The IsBusy attached property for a anything that wants to flag if the control is busy
    /// </summary>
    public class IsBusyProperty : BaseAttachedProperty<IsBusyProperty, bool>
    {
    }

    /// <summary>
    /// The IsLoggedIn attached property for a anything that wants to flag if authentication fails
    /// </summary>
    public class IsLoggedIn : BaseAttachedProperty<IsLoggedIn, bool>
    {
    }
}
