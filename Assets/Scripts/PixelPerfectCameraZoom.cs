using UnityEngine;
using UnityEngine.U2D;

public class PixelPerfectCameraZoom : MonoBehaviour {
    private static int MAX_PPU = 50;
    private static int MIN_PPU = 7;

    public new PixelPerfectCamera camera;

    public void OnGUI() {
        Event e = Event.current;

        // Update camera Pixel-Per-Unit by change in y,
        // which includes magnitude (force) of scroll wheel
        if (e.isScrollWheel) {
            camera.assetsPPU = CalculatePPU((int) e.delta.y);
        }
    }

    /// <summary>
    /// Given the magnitude of a scroll wheel, calculate the PPU (with min/max) boundaries
    /// </summary>
    /// <param name="scrollForce">magnitude of scroll wheel</param>
    /// <returns></returns>
    private int CalculatePPU(int scrollForce) {
        int newPPU = camera.assetsPPU + (scrollForce * -1);

        if (newPPU > MAX_PPU) {
            newPPU = MAX_PPU;
        }

        if (newPPU < MIN_PPU) {
            newPPU = MIN_PPU;
        }

        return newPPU;
    }
}