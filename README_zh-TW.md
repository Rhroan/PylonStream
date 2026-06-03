# Pylon RTSP & WebRTC Multi-Camera Streamer (繁體中文說明書)

本專案是一個結合 **Windows Forms 本地控制介面** 與 **Self-Hosted ASP.NET Core Kestrel 網頁伺服器** 的工業級多相機影像串流系統。專門設計用於將 **Basler GigE 工業相機** 的高解析度影像，以極低延遲（WebRTC <500ms，RTSP <1s）串流至網路中，方便網頁端、影像演算法端（如 OpenCV, Python）或第三方串流伺服器接收。

---

## 🏗️ 系統架構圖

```
                    ┌────────────────────────────────────────┐
                    │      Windows Forms 本地主控程序         │
                    │ ┌──────────────┐      ┌──────────────┐ │
                    │ │ MainForm UI  │      │ SettingsForm │ │
                    │ └──────┬───────┘      └──────┬───────┘ │
                    └────────┼─────────────────────┼─────────┘
                             │  ┌───────────────┐  │
                             └─►│ Modules.Config│◄─┘
                                └───────┬───────┘
                                        ▼
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │                       Kestrel 背景 Web 伺服器 (Port 5000)                    │
 │  ┌─────────────────────────────────┐   ┌─────────────────────────────────┐  │
 │  │     ApiController (REST API)    │   │     StreamHub (SignalR Hub)     │  │
 │  │ - 提供狀態、網卡列表、重啟、存檔 │   │ - 向 Web SPA 廣播即時系統 Log   │  │
 │  └─────────────────────────────────┘   └─────────────────────────────────┘  │
 └──────────────────────────────────────┬──────────────────────────────────────┘
                                        ▼
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │                          wwwroot (SPA Web Dashboard)                        │
 │  ┌─────────────────────────────────┐   ┌─────────────────────────────────┐  │
 │  │      Dashboard (WHEP 播放器)    │   │      Settings (全域與相機設定)   │  │
 │  └─────────────────────────────────┘   └─────────────────────────────────┘  │
 └──────────────────────────────────────┬──────────────────────────────────────┘
                                        ▼
 ┌─────────────────────────────────────────────────────────────────────────────┐
 │                         相機管線管理器 (CameraManager)                       │
 │  ┌───────────────────────────────────────────────────────────────────────┐  │
 │  │                    CameraStreamPipeline (每個 IP 一個執行緒)          │  │
 │  │  ┌────────────┐   ┌────────────┐   ┌────────────┐   ┌──────────────┐  │  │
 │  │  │ Pylon 取像 │──►│ RGB/Mono   │──►│ FFmpeg     │──►│ MediaMTX     │  │  │
 │  │  │ (64 Buffer)│   │ 零拷貝轉碼 │   │ Stdin Pipe │   │ RTSP/WebRTC  │  │  │
 │  │  └────────────┘   └────────────┘   └────────────┘   └──────────────┘  │  │
 │  └───────────────────────────────────────────────────────────────────────┘  │
 └─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🌟 核心特點

1. **雙核心運行架構**：
   - WinForms 與 Web API / WebSockets 共享全域狀態 `Modules`。可以在本機視窗控制，也能透過網路遠端在網頁中控台管理與預覽。
2. **高防禦性取像迴圈**：
   - 使用 Pylon SDK 的 `RetrieveResult` 無限擷取模式，避免固定張數後中斷。
   - 驅動緩衝區擴增至 **`64` 個 Buffers**（設定 `PLStream.MaxNumBuffer`），搭配 C# `Span<T>` 與 `ArrayPool` 進行像素級處理，有效應對 GC 停頓防止掉包。
   - 偵測到斷線或 `RetrieveResult` 為 Null 時自動觸發 Exception 釋放資源，5 秒後自動重連。
3. **FFmpeg 硬體加速編碼**：
   - 支援 H.264 與 H.265 編碼。
   - 支援 **CPU (軟解)、NVIDIA GPU (NVENC)、Intel QuickSync (QSV)** 三種編碼器模式。
4. **關鍵影格 (GOP) 最佳化（WebRTC 零延遲秒開）**：
   - 移除了會導致 Chrome 等瀏覽器解碼卡死的 `-tune zerolatency`（內部更新掃描）。
   - 強制配置 `-g {TargetFps} -bf 0`，每 1 秒產生一個獨立關鍵影格且無 B 幀。網頁端 WebRTC 載入後 1 秒內必定出圖，延遲低於 500ms。
5. **網路介面（Network Interface）綁定隔離**：
   - 可在設定中指定特定的網路卡 IP（如 `192.168.1.56`）。
   - **串流流量** (MediaMTX) 會嚴格綁定在該 IP，隔離工業相機內部網段與辦公室網路。
   - **管理服務** (Kestrel) 保持監聽 `0.0.0.0`，確保 `localhost:5000` 環迴連線與外部實體 IP 連線皆能正常使用。

---

## 🛠️ 新電腦建置與運行指南 (New PC Setup Guide)

當您下載此專案原始碼，並打算在一台全新的 Windows 電腦上編譯與執行時，請務必按照以下步驟進行設定：

### 步驟 1：安裝 .NET 8.0 SDK
- 本專案採用 .NET 8.0 開發。
- 請前往 [微軟官方下載點](https://dotnet.microsoft.com/zh-tw/download/dotnet/8.0) 安裝 **.NET SDK 8.0**。

### 步驟 2：安裝 Basler Pylon SDK (專案編譯依賴)
- 本專案在 `PylonStream.csproj` 中，靜態參考了本機 Pylon 6 安裝路徑下的組件：
  `C:\Program Files\Basler\pylon 6\Development\Assemblies\Basler.Pylon\x64\Basler.Pylon.dll`
- **操作步驟**：
  1. 請下載並安裝 [Basler pylon Camera Software Suite for Windows](https://www.baslerweb.com/) (建議安裝 **pylon 6**)。
  2. 確保 Pylon 6 的 DLL 存在於上述預設路徑。
  3. *(若使用 pylon 7 或其他版本，請在 Visual Studio 中手動重新加入 `Basler.Pylon` 參考，或編輯 `.csproj` 檔案中的 `<HintPath>` 修改為對應版本的 DLL 位置)*。

### 步驟 3：下載 FFmpeg 與 MediaMTX 執行檔 (串流引擎)
由於執行檔體積過大且常變更，Git 儲存庫並未包含這兩個 `.exe` 檔案。您需要手動下載並配置：
1. **FFmpeg**：
   - 前往 [FFmpeg Gyan.dev 平台](https://www.gyan.dev/ffmpeg/builds/) 或 [GitHub 釋出頁面](https://github.com/GyanD/codexffmpeg/releases) 下載 Windows 64-bit 靜態建置版本（`ffmpeg-git-full.7z`）。
   - 解壓縮後，提取 **`ffmpeg.exe`**。
2. **MediaMTX**：
   - 前往 [MediaMTX GitHub Releases](https://github.com/bluenviron/mediamtx/releases) 下載 Windows amd64 版本（`mediamtx_vX.Y.Z_windows_amd64.zip`）。
   - 解壓縮後，提取 **`mediamtx.exe`**。

### 步驟 4：建立編譯輸出資料夾並放置依賴檔
1. 在專案根目錄下，開啟 PowerShell 執行建置命令：
   ```powershell
   dotnet build
   ```
2. 編譯成功後，前往編譯輸出目錄（預設為 `bin\Debug\net8.0-windows\`）。
3. 在該目錄下**手動建立名為 `Binaries` 的資料夾**。
4. 將剛才下載的 **`ffmpeg.exe`** 與 **`mediamtx.exe`** 放入 `Binaries/` 中。
   *(目錄結構應如下所示)*：
   ```
   PylonStream/
     └── bin/
         └── Debug/
             └── net8.0-windows/
                 ├── PylonStream.exe
                 ├── wwwroot/              <-- (網頁前端靜態檔，編譯時會自動複製)
                 └── Binaries/             <-- (手動建立)
                     ├── ffmpeg.exe        <-- (手動放置)
                     └── mediamtx.exe      <-- (手動放置)
   ```

### 步驟 5：執行與配置相機 IP
1. 雙擊執行 `PylonStream.exe`。
2. 點擊主畫面的 **「Settings...」** 開啟設定頁：
   - 設定您的相機實體 IP Address。
   - 在 **Bind Network Interface** 下拉選單中選取您的專用相機網卡 IP。
3. 點選 **「Save Configuration」** 保存，軟體會在同目錄下自動產生 `config.json` 設定檔。

---

## ⚡ 工業相機網卡硬體最佳化 (GigE Optimization)

為了讓 GigE 相機在高解析度（5MP / 2448x2048）多相機環境下穩定傳輸且不掉包，請在新電腦中進行以下網卡設定：
1. 開啟 **裝置管理員** ➡️ 展開 **網路介面卡** ➡️ 在與相機連接的網路卡上點選右鍵 ➡️ **內容**。
2. 切換至 **進階 (Advanced)** 頁籤：
   - **Jumbo Packet (巨型框架 / 巨型封包)**：修改為 **9014 Bytes** 或 **9000 Bytes**（若網卡與交換器支援）。並同步在軟體的相機設定中，將 Packet Size 修改為相同值。
   - **Receive Buffers (接收緩衝區)**：將數值調至最大值（例如 **2048** 或 **4096**）。
   - **Energy Efficient Ethernet (節能乙太網路)**：修改為 **Disabled (停用)**，防止相機在低 FPS 運作時網卡自動休眠導致連線中斷。

---

## 🚀 運行與操作

1. **開啟控制台網頁**：
   - 瀏覽器訪問：`http://localhost:5000` (或區域網路中的 `http://<C#_Server_IP>:5000`)。
2. **啟動串流服務**：
   - 點選右上角的 **「▶ Start Service」**。
   - 等待相機 Status 轉為綠色的 **`Grabbing`**。
3. **即時畫面預覽**：
   - 在相機列表中點選任意相機列，右側的 **Live WHEP Preview** 面板會自動連線並載入 WebRTC 超低延遲畫面！

---

## 📡 串流接收方式

### 1. RTSP 串流（VLC / OpenCV / Python / 影像演算法）
- **格式**：`rtsp://<C#_Server_IP>:8554/<StreamName>`
- **Python OpenCV 連接範例**：
  ```python
  import cv2
  
  # 連接至 cam1 串流
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

### 2. WebRTC / WHEP 串流（網頁極低延遲嵌入）
- **格式**：`http://<C#_Server_IP>:8889/<StreamName>`
- 網頁端可以直接以 `<iframe>` 嵌入該連結，MediaMTX 內建 WHEP 播放器會自動完成 WebRTC 連線與渲染。

---

## 🌐 REST API 參考

其他第三方系統可以透過標準 HTTP 請求來監控與操作本系統：

| 方法 | 路由 | 描述 |
| :--- | :--- | :--- |
| **GET** | `/api/status` | 取得伺服器運行狀態、MediaMTX 狀態、網卡 IP 及所有相機的即時解析度與當前 FPS。 |
| **GET** | `/api/interfaces` | 獲取目前本機所有 Up 狀態的實體網路卡介面列表（包含 IP 與網卡描述）。 |
| **GET** | `/api/config` | 獲取目前程式使用的完整 `config.json` 內容快照。 |
| **GET** | `/api/logs` | 獲取最新 200 行系統日誌。 |
| **POST** | `/api/control/start` | 啟動 MediaMTX 及所有相機的影像擷取/推流管線。 |
| **POST** | `/api/control/stop` | 停止所有相機管線並終止 MediaMTX 進程。 |
| **POST** | `/api/control/restart-camera?ip={ip}` | 單獨重新啟動某台指定 IP 相機的影像擷取管線。 |
| **POST** | `/api/config` | 儲存並覆寫 `config.json` 參數（Body 需帶入完整的 AppConfig JSON 結構）。 |

---

## 🛠️ 故障排除 (Troubleshooting)

- **狀態卡在 "Reconnecting" 或 Log 出現 "controlled by another application (0xE1018006)"**：
  - 代表該相機 IP 被本機的其他 Pylon 程式（例如 Pylon Viewer）獨占。請關閉其他程式，本系統會在 5 秒後自動重連成功。
- **畫面卡在 "Loading / 轉圈圈"**：
  - 舊的 `mediamtx.exe` 或 `ffmpeg.exe` 異常中斷並佔用通訊埠。請在 Windows 命令提示字元執行以下指令清理背景殘留：
    ```powershell
    taskkill /F /IM mediamtx.exe; taskkill /F /IM ffmpeg.exe
    ```
    隨後重啟 C# 程式即可恢復正常。
- **影像出現綠線、破圖或頻繁 complete grab 警告**：
  - 網路頻寬不足或交換器掉包。請參閱 **網卡硬體最佳化** 步驟開啟 Jumbo Frames，並在相機設定中適當增加 **Inter-Packet Delay**（建議設定為 `3000` ~ `4500`）以平滑封包流量。
