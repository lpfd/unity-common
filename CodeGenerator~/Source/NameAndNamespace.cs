namespace Leap.Forward.Unity.Common
{
    public class NameAndNamespace
    {
        public NameAndNamespace(string fullName)
        {
            var lastDot = fullName.LastIndexOf('.');
            Name = fullName.Substring(lastDot + 1);
            if (lastDot > 0)
            {
                Namespace = fullName.Substring(0, lastDot);
            }
        }

        public string Name { get; set; }

        public string Namespace { get; set; }
    }

}