namespace Divergic.Logging.Xunit.UnitTests
{
    public class CircularReference
    {
        public CircularReference()
        {
            Self = this;
        }

        public CircularReference Self { get; set; }
    }
}