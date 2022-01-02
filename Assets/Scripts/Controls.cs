// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace DM
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""88949387-ec0a-42ca-99b6-bd61f8fd3b2c"",
            ""actions"": [
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Button"",
                    ""id"": ""88ae28de-b95f-4663-bf55-4ac6576fac57"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""Button"",
                    ""id"": ""d3a235bc-7f04-41f1-b889-c850ffb99209"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""1bcab131-210e-47f2-ae73-f0de605a071b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""14a71a01-f593-4cd3-98a8-0d29a8572f11"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f00a23c1-7a08-4770-9afc-a14ae464e2fc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""f3accb27-1748-4dd6-801e-4bd284803d6d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""caac17f4-1556-4969-a5c3-8fd9903267cf"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""48c43ef6-8ec7-4611-af3f-76dc92d6eb03"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_Vertical = m_Gameplay.FindAction("Vertical", throwIfNotFound: true);
            m_Gameplay_Horizontal = m_Gameplay.FindAction("Horizontal", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_Vertical;
        private readonly InputAction m_Gameplay_Horizontal;
        public struct GameplayActions
        {
            private @Controls m_Wrapper;
            public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Vertical => m_Wrapper.m_Gameplay_Vertical;
            public InputAction @Horizontal => m_Wrapper.m_Gameplay_Horizontal;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @Vertical.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVertical;
                    @Vertical.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVertical;
                    @Vertical.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnVertical;
                    @Horizontal.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontal;
                    @Horizontal.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontal;
                    @Horizontal.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHorizontal;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Vertical.started += instance.OnVertical;
                    @Vertical.performed += instance.OnVertical;
                    @Vertical.canceled += instance.OnVertical;
                    @Horizontal.started += instance.OnHorizontal;
                    @Horizontal.performed += instance.OnHorizontal;
                    @Horizontal.canceled += instance.OnHorizontal;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);
        public interface IGameplayActions
        {
            void OnVertical(InputAction.CallbackContext context);
            void OnHorizontal(InputAction.CallbackContext context);
        }
    }
}
