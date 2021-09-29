using UnityEngine;
using Photon.Pun;

namespace ChiliGames.VROffice
{
    //This class sends a Raycast from the marker and detect if it's hitting the whiteboard (tag: Finish)
    public class Marker : MonoBehaviour
    {
        private Whiteboard whiteboard;
        public Transform drawingPoint;
        public Renderer markerTip;
        private RaycastHit touch;
        bool touching;
        float drawingDistance = 0.015f;
        Quaternion lastAngle;
        PhotonView pv;
        [SerializeField] int penSize = 6;
        [SerializeField] Color color = Color.blue;
        bool grabbed;

        public void ToggleGrab(bool b)
        {
            if (b) grabbed = true;
            else grabbed = false;
        }

        private void Start()
        {
            pv = GetComponent<PhotonView>();
            var block = new MaterialPropertyBlock();

            // You can look up the property by ID instead of the string to be more efficient.
            block.SetColor("_BaseColor", color);

            // You can cache a reference to the renderer to avoid searching for it.
            markerTip.SetPropertyBlock(block);
        }

        void Update()
        {
            //if the marker is not in possesion of the user, or is not grabbed, we don't run update.
            if (!pv.IsMine) return;
            if (!grabbed) return;

            //Cast a raycast to detect whiteboard.
            if (Physics.Raycast(drawingPoint.position, drawingPoint.up, out touch, drawingDistance))
            {
                //The whiteboard has the tag "Finish".
                if (touch.collider.CompareTag("Finish"))
                {
                    if (!touching)
                    {
                        touching = true;
                        lastAngle = transform.rotation;
                        whiteboard = touch.collider.GetComponent<Whiteboard>();
                    }
                    if (whiteboard == null) return;
                    //Send the rpc with the coordinates, pen size and color of marker in RGB.
                    whiteboard.pv.RPC("DrawAtPosition", RpcTarget.AllBuffered, new float[] { touch.textureCoord.x, touch.textureCoord.y }, penSize, new float[]{color.r, color.g, color.b});
                }
            }
            else if (whiteboard != null)
            {
                touching = false;
                whiteboard.pv.RPC("ResetTouch", RpcTarget.AllBuffered);
                whiteboard = null;
            }
        }

        private void LateUpdate()
        {
            if (!pv.IsMine) return;

            //lock rotation of marker when touching whiteboard.
            if (touching)
            {
                transform.rotation = lastAngle;
            }
        }
    }
}

