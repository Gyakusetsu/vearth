#if UNITY_EDITOR
namespace Vearth {
    public abstract class VearthTab {

        public string Description;

        public VearthTab(string description) {
            Description = description;
        }

        public abstract void ShowTabContent();
    }
}
#endif