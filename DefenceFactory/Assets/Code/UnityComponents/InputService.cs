using Leopotam.Ecs.Types;
using UnityEngine;

namespace DefenceFactory
{
    class InputService : MonoBehaviour, IInputService
    {
        [SerializeField] private Camera _camera;

        bool IInputService.GetClickedCoordinate(out Int2 coord)
        {
            coord = new Int2();
            if (!Input.GetMouseButtonUp(0))
                return false;
            var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            coord.Set(
                Mathf.FloorToInt(worldPosition.x),
                Mathf.FloorToInt(worldPosition.y)
            );
            return true;
        }

        private bool _mouseDown = false;
        bool IInputService.GetCursorCoordinate(out Int2 coord)
        {
            coord = new Int2();

            if (Input.GetMouseButtonDown(0))
            {
                _mouseDown = true;
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                _mouseDown = false;
            }

            if (_mouseDown)
            {
                var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                coord.Set(
                    Mathf.FloorToInt(worldPosition.x),
                    Mathf.FloorToInt(worldPosition.y)
                );
                return true;
            }
            return false;
        }
    }
}
