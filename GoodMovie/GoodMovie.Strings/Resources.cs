using Windows.ApplicationModel.Resources;

namespace GoodMovie.Strings 
{
    public class Resources 
    {
        public ResourceLoader ResourceLoader  {get;}

        public Resources() 
        {
            ResourceLoader = ResourceLoader.GetForCurrentView("GoodMovie.Strings/Resources");
        }

 

        public string Actor => ResourceLoader.GetString(nameof(Actor));    
        public string Film => ResourceLoader.GetString(nameof(Film));    
    }
}
