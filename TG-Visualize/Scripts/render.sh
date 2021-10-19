EDGE_PATH="/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge"
DLL_PATH="/Users/antonio/Local Apps/TG-Visualize/TG-Visualize.dll"
OUT_DIR="Rendered"

[ -d $OUT_DIR ] || mkdir $OUT_DIR

for file in *.dat; do
    dotnet "$DLL_PATH" $file
    "$EDGE_PATH" --headless --disable-gpu --screenshot="$OUT_DIR/${file%.*}.jpeg" --window-size=1200,900 "${file%.*}.html"
done
