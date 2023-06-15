using SunSharp.ObjectWrapper;
using SunSharp.ObjectWrapper.Modules;
using UnityEngine;

namespace TopDownShooter
{
    public class MusicController : MonoBehaviour
    {
        [SerializeReference] SunSharp.Unity.SongAsset _song;
        private SunVox _sunVox;
        private Slot _slot;
        private FilterProModuleHandle _filterHandle;
        [SerializeField] private bool _shouldBeFiltered = true;
        public bool ShouldBeFiltered { get => _shouldBeFiltered; set => _shouldBeFiltered = value; }

        private void Awake()
        {
            _sunVox = new SunVox(SunSharp.Unity.Library.Instance);
            _slot = _sunVox.Slots[0];
            _slot.Open();
            _slot.Load(_song.Data);
            _slot.PlayFromBeginning();
            _slot.Synthesizer.TryGetModule("OutputFilter", out var handle);
            _filterHandle = handle.AsFilterPro();
        }

        private void Update()
        {
            _filterHandle.SetFreq(_shouldBeFiltered ? 159 : 22000);
        }
    }
}