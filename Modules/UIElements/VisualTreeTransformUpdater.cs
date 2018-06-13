// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace UnityEngine.Experimental.UIElements
{
    internal class VisualTreeTransformUpdater : BaseVisualTreeUpdater
    {
        private uint m_Version = 0;
        private uint m_LastVersion = 0;

        public override string description
        {
            get { return "Update Transform"; }
        }

        public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
        {
            if ((versionChangeType & VersionChangeType.Transform) != VersionChangeType.Transform)
                return;

            if (ve.isWorldTransformDirty)
                return;

            ++m_Version;

            ve.isWorldTransformDirty = true;

            PropagateToChildren(ve);
        }

        private void PropagateToChildren(VisualElement ve)
        {
            int count = ve.shadow.childCount;
            for (int i = 0; i < count; i++)
            {
                var child = ve.shadow[i];
                if (child.isWorldTransformDirty)
                    continue;

                child.isWorldTransformDirty = true;
                PropagateToChildren(child);
            }
        }

        public override void Update()
        {
            if (m_Version == m_LastVersion)
                return;

            m_LastVersion = m_Version;

            // Update element under mouse
            EventDispatcher eventDispatcher = UIElementsUtility.eventDispatcher as EventDispatcher;
            if (eventDispatcher != null)
            {
                eventDispatcher.UpdateElementUnderMouse(panel);
            }
        }
    }
}