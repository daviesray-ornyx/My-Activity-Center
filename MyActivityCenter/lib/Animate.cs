using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MyActivityCenter.lib
{
    // The Singleton class defines the `GetInstance` method that serves as an
    // alternative to constructor and lets clients access the same instance of
    // this class over and over.
    class Animate
    {
        // The Singleton's constructor should always be private to prevent
        // direct construction calls with the `new` operator.
        private Animate() { }

        // The Singleton's instance is stored in a static field. There there are
        // multiple ways to initialize this field, all of them have various pros
        // and cons. In this example we'll show the simplest of these ways,
        // which, however, doesn't work really well in multithreaded program.
        private static Animate _instance;

        // This is the static method that controls the access to the singleton
        // instance. On the first run, it creates a singleton object and places
        // it into the static field. On subsequent runs, it returns the client
        // existing object stored in the static field.
        public static Animate GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Animate();
            }
            return _instance;
        }

        // Finally, any singleton should define some business logic, which can
        // be executed on its instance.
        public static void someBusinessLogic()
        {
            // Get some business logic here
        }

        public DoubleAnimation GetMenuDoubleAnimation(double from, double to, double durationInMilliseconds, double speedRation)
        {
            return new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(durationInMilliseconds),
                SpeedRatio = speedRation
            };
        }
    }
}
