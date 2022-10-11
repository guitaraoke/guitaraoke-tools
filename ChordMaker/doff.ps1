ffmpeg `
    -i original.mp4 `
     -c:v libvpx-vp9 `
    -i overlay.webm  `
    -filter_complex "[0:0]setpts=1001*N/30000/TB[src];
        [1:0]setpts=1001*N/30000/TB[ovl];
        [src][ovl]overlay" `
    -c:a copy `
    -ss 00:00:00 `
    -t 00:00:10 `
    -y -c:v libx264 overlaid.mp4


    # -filter_complex "[0:0]setpts=N/FRAME_RATE/TB[src];[1:0]setpts=N/FRAME_RATE/TB[ovl];[src][ovl]overlay" `