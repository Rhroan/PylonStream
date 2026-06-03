# Pylon RTSP & WebRTC Multi-Camera Streamer

This project is an industrial-grade multi-camera video streaming system that combines a **local Windows Forms control interface** and a **Self-Hosted ASP.NET Core Kestrel web server**. It is designed to stream high-resolution images from **Basler GigE industrial cameras** over the network with ultra-low latency (WebRTC <500ms, RTSP <1s), facilitating consumption by web front-ends, image processing algorithms (e.g., OpenCV, Python), or third-party streaming servers.

---

## 🏗️ System Architecture

```
                    ┌────────────────────────────────────────┐
                    │      Windows Forms Local Control       │
                    │ ┌──────────────┐      ┌──────────────┐ │
                    │ │ MainForm UI  │      │ SettingsForm │ │
                    │ └──────┬───────┘      └──────┬───────┘ │
                    └────────┼─────────────────────┼─────────┘
                             │  ┌───────────────┐  │
                             └─►│ Modules.Config│◄─┘
                                └───────┬───────┘
                                        ▼
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │                    Kestrel Background Web Server (Port 5000)                │
 │  ┌─────────────────────────────────┐   ┌─────────────────────────────────┐  │
 │  │     ApiController (REST API)    │   │     StreamHub (SignalR Hub)     │  │
 │  │ - Status, network interfaces,   │   │ - Broadcasts real-time system   │  │
 │  │   reloading & saving configs    │   │   logs to Web SPA               │  │
 │  └─────────────────────────────────┘   └─────────────────────────────────┘  │
 └──────────────────────────────────────┬──────────────────────────────────────┘
                                        ▼
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │                         wwwroot (SPA Web Dashboard)                         │
 │  ┌─────────────────────────────────┐   ┌─────────────────────────────────┐  │
 │  │      Dashboard (WHEP Player)    │   │  Settings (Global & Camera)     │  │
 │  └─────────────────────────────────┘   └─────────────────────────────────┘  │
 └──────────────────────────────────────┬──────────────────────────────────────┘
                                        ▼
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │                         Camera Pipeline Manager (CameraManager)             │
 │  ┌───────────────────────────────────────────────────────────────────────┐  │
 │  │                    CameraStreamPipeline (One thread per IP)           │  │
 │  │  ┌────────────┐   ┌────────────┐   ┌────────────┐   ┌──────────────┐  │  │
 │  │  │   Pylon    │──►│ RGB / Mono │──►│   FFmpeg   │──►│   MediaMTX   │  │  │
 │  │  │  Grabbing  │   │ Zero-Copy  │   │ Stdin Pipe │   │ RTSP/WebRTC  │  │  │
 │  │  │ (64 Buffer)│   │ Conversion │   │            │   │              │  │  │
 │  │  └────────────┘   └────────────┘   └────────────┘   └──────────────┘  │  │
 │  └───────────────────────────────────────────────────────────────────────┘  │
 └─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🌟 Core Features

1. **Dual-Core Architecture**:
   - WinForms and Web API / WebSockets share a global state `Modules`. It can be controlled locally via windows or managed and previewed remotely via a web control dashboard.
2. **Highly Robust Grab Loop**:
   - Uses Pylon SDK's `RetrieveResult` infinite grab mode to prevent interruption after capturing a fixed number of frames.
   - Driver buffer pool is expanded to **`64` buffers** (`PLStream.MaxNumBuffer`), using C# `Span<T>` and `ArrayPool` for pixel-level operations to absorb CPU/GC pauses and prevent packet drops.
   - Automatically throws an exception to release resources when a disconnect or null `RetrieveResult` is detected, and attempts to reconnect automatically after 5 seconds.
3. **FFmpeg Hardware-Accelerated Encoding**:
   - Supports H.264 and H.265 codecs.
   - Supports three encoder modes: **CPU (Software), NVIDIA GPU (NVENC), and Intel QuickSync (QSV)**.
4. **GOP Optimization (WebRTC Sub-second Startup)**:
   - Removed `-tune zerolatency` (which uses periodic intra-refresh that freezes Chrome's WebRTC decoder).
   - Explicitly configures `-g {TargetFps} -bf 0` to force a discrete keyframe (I-frame) every 1 second without B-frames. WebRTC loads and displays video within 1 second with latency under 500ms.
5. **Network Interface Binding & Isolation**:
   - Allows specifying a particular network interface IP (e.g., `192.168.1.56`).
   - **Streaming Traffic** (MediaMTX) is strictly bound to this IP, isolating the camera network from the general office network.
   - **Management Interface** (Kestrel) continues to listen on `0.0.0.0`, ensuring that loopback (`localhost:5000`) and external real-world IP connections remain fully accessible.

---

## 🛠️ New PC Setup Guide

When you download/clone this repository and plan to compile and execute it on a completely new Windows machine, make sure to configure it as follows:

### Step 1: Install .NET 8.0 SDK
- This project is built on .NET 8.0.
- Visit [Microsoft Official .NET Download Page](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) and install **.NET SDK 8.0**.

### Step 2: Install Basler Pylon SDK (Project Compilation Dependency)
- This project statically references the assembly file located in the local Pylon 6 installation directory within `PylonStream.csproj`:
  `C:\Program Files\Basler\pylon 6\Development\Assemblies\Basler.Pylon\x64\Basler.Pylon.dll`
- **Steps to resolve**:
  1. Download and install [Basler pylon Camera Software Suite for Windows](https://www.baslerweb.com/) (Version **pylon 6** is recommended).
  2. Verify that `Basler.Pylon.dll` exists in the above default path.
  3. *(Note: If you use pylon 7 or other versions, please manually remove and re-add the `Basler.Pylon` reference in Visual Studio, or edit the `<HintPath>` in the `.csproj` file to match your installed version)*.

### Step 3: Download FFmpeg and MediaMTX Binaries (Streaming Engine)
Since these executable files are large and frequently updated, they are excluded from this Git repository. You need to download them manually:
1. **FFmpeg**:
   - Go to [FFmpeg Gyan.dev Build](https://www.gyan.dev/ffmpeg/builds/) or the [GitHub Release Page](https://github.com/GyanD/codexffmpeg/releases) to download the Windows 64-bit static build (e.g., `ffmpeg-git-full.7z`).
   - Extract and retrieve **`ffmpeg.exe`**.
2. **MediaMTX**:
   - Go to the [MediaMTX GitHub Releases](https://github.com/bluenviron/mediamtx/releases) page to download the Windows amd64 version (`mediamtx_vX.Y.Z_windows_amd64.zip`).
   - Extract and retrieve **`mediamtx.exe`**.

### Step 4: Create Compilation Output Directories and Place Executables
1. Open PowerShell in the project root directory and compile the application:
   ```powershell
   dotnet build
   ```
2. Navigate to the compiled output directory (default path is `bin\Debug\net8.0-windows\`).
3. **Manually create a folder named `Binaries`** inside that directory.
4. Copy the downloaded **`ffmpeg.exe`** and **`mediamtx.exe`** into the newly created `Binaries/` folder.
   *(The final directory structure should look exactly like this)*:
   ```
   PylonStream/
     └── bin/
         └── Debug/
             └── net8.0-windows/
                 ├── PylonStream.exe
                 ├── wwwroot/              <-- (Static Web App assets, copied automatically)
                 └── Binaries/             <-- (Manually Created)
                     ├── ffmpeg.exe        <-- (Manually Placed)
                     └── mediamtx.exe      <-- (Manually Placed)
   ```

### Step 5: Run and Configure Camera IPs
1. Double-click to execute `PylonStream.exe`.
2. Click **「Settings...」** on the main WinForms window:
   - Configure your camera's physical IP addresses.
   - Choose your camera-facing network card IP in the **Bind Network Interface** dropdown.
3. Click **「Save Configuration」** to persist settings. A `config.json` file will be generated automatically in the application directory.

---

## ⚡ GigE Network Adapter Hardware Optimization

To ensure stable data transmission and zero packet drops (`Incomplete Grab`) for high-resolution GigE cameras (such as 5MP / 2448x2048) in multi-camera configurations, perform the following settings on your Windows machine:
1. Open **Device Manager** ➡️ expand **Network Adapters** ➡️ right-click the network card connected to the cameras ➡️ select **Properties**.
2. Under the **Advanced** tab:
   - **Jumbo Packet (Jumbo Frames)**: Set to **9014 Bytes** or **9000 Bytes** (if supported by network card and switch). Match this value in the application's camera Settings (`Packet Size`).
   - **Receive Buffers**: Set to the maximum supported value (e.g., **2048** or **4096**).
   - **Energy Efficient Ethernet (Green/Eco Ethernet)**: Set to **Disabled** to prevent the network card from sleeping during low-frame rate operations, causing stream dropouts.

---

## 🚀 Run & Operation

1. **Access control dashboard via browser**:
   - Navigate to: `http://localhost:5000` (or `http://<C#_Server_IP>:5000` over local network).
2. **Start Services**:
   - Click the **「▶ Start Service」** button on the top-right of the web page.
   - Wait for the cameras' status to turn green **`Grabbing`**.
3. **Live Web Preview**:
   - Click on any camera row in the table, the **Live WHEP Preview** panel on the right will automatically establish connection and stream the live WebRTC video!

---

## 📡 How to Consume the Stream

### 1. RTSP Stream (VLC / OpenCV / Python / Video Analytics)
- **Format**: `rtsp://<C#_Server_IP>:8554/<StreamName>`
- **Python OpenCV Connection Example**:
  ```python
  import cv2
  
  # Connect to cam1 stream
  cap = cv2.VideoCapture("rtsp://192.168.1.56:8554/cam1")
  
  while cap.isOpened():
      ret, frame = cap.read()
      if not ret:
          break
      cv2.imshow('Live Camera 1', frame)
      if cv2.waitKey(1) & 0xFF == ord('q'):
          break
  cap.release()
  cv2.destroyAllWindows()
  ```

### 2. WebRTC / WHEP Stream (Web Integration)
- **Format**: `http://<C#_Server_IP>:8889/<StreamName>`
- Web pages can embed this link inside an `<iframe>`. The built-in player inside MediaMTX will handle WebRTC SDP signaling and render video out-of-the-box.

---

## 🌐 REST API Reference

Third-party systems can query and control the streamer using standard HTTP REST requests:

| Method | Route | Description |
| :--- | :--- | :--- |
| **GET** | `/api/status` | Gets server state, MediaMTX state, network interface IP, and current camera metrics (resolution, live FPS, status). |
| **GET** | `/api/interfaces` | Retrieves a list of active network adapter interfaces (IPs and card descriptions). |
| **GET** | `/api/config` | Retrieves the current `config.json` configuration file payload. |
| **GET** | `/api/logs` | Gets the 200 most recent lines of console logs. |
| **POST** | `/api/control/start` | Starts the MediaMTX process and triggers Pylon grab loops for all cameras. |
| **POST** | `/api/control/stop` | Stops all camera loops and terminates the MediaMTX process cleanly. |
| **POST** | `/api/control/restart-camera?ip={ip}` | Restarts a single specified camera capture pipeline. |
| **POST** | `/api/config` | Overwrites and updates `config.json` configurations (requires AppConfig JSON body payload). |

---

## 🛠️ Troubleshooting

- **Status stuck in "Reconnecting" or Logs showing "controlled by another application (0xE1018006)"**:
  - The camera is locked by another program (e.g., Basler pylon Viewer). Close other software; the application will auto-reconnect successfully in 5 seconds.
- **Video player stuck at "Loading / Spinning"**:
  - A previous instance of `mediamtx.exe` or `ffmpeg.exe` was forced to terminate but remained as a zombie process. Execute the following in PowerShell/Command Prompt to clear them:
    ```powershell
    taskkill /F /IM mediamtx.exe; taskkill /F /IM ffmpeg.exe
    ```
    Then restart the C# application.
- **Green lines, pixel corruption, or frequent complete grab warnings**:
  - Insufficient network bandwidth or network adapter collisions. Refer to the **Hardware Optimization** section to enable Jumbo Frames, and increase **Inter-Packet Delay** (e.g., `3000` to `4500`) in configuration to smooth traffic flow.
