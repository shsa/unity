using Leopotam.Ecs.Types;
using System;
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

        private DateTime _mouseDownTime;
        private Vector3 _startScreenPos;
        private Vector3 _startWorldPos;
        void UpdateInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDownTime = DateTime.Now;
                _startScreenPos = Input.mousePosition;
                _startWorldPos = _camera.ScreenToWorldPoint(_startScreenPos);
            }
        }

        bool IInputService.GetCursorCoordinate(out Int2 coord)
        {
            UpdateInput();

            coord = new Int2();

            if (Input.GetMouseButton(0))
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

        bool IInputService.GetShift(out Float2 coord)
        {
            UpdateInput();
            if (Input.GetMouseButton(0))
            {
                var worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                coord = new Float2(worldPos.x - _startWorldPos.x, worldPos.y - _startWorldPos.y);
                return true;
            }
            else
            {
                coord = default;
                return false;
            }
        }

        Float2 ScreenToWorldFloat2(Vector3 screenPoint)
        {
            var worldPos = _camera.ScreenToWorldPoint(screenPoint);
            return new Float2(worldPos.x, worldPos.y);
        }

        bool IInputService.GetClicked(out Float2 coord)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if ((DateTime.Now - _mouseDownTime).TotalMilliseconds < 500)
                {
                    coord = ScreenToWorldFloat2(Input.mousePosition);
                    return true;
                }
            }

            coord = default;
            return false;
        }

        private Float2 _startWorldPosFloat2;
        bool IInputService.GetDrag(out Float2 coord, out DefenceFactory.Ecs.DragEnum state)
        {
            UpdateInput();
            if (Input.GetMouseButtonDown(0))
            {
                state = Ecs.DragEnum.Begin;
                coord = ScreenToWorldFloat2(Input.mousePosition);
                _startWorldPosFloat2 = coord;
                return true;
            }

            if (Input.GetMouseButton(0))
            {
                state = Ecs.DragEnum.Current;
                var c0 = ScreenToWorldFloat2(_startScreenPos);
                var offset = c0 - _startWorldPosFloat2;
                coord = ScreenToWorldFloat2(Input.mousePosition) - offset;
                return true;
            }
            else
            {
                state = Ecs.DragEnum.None;
                coord = default;
                return false;
            }
        }
    }
}
