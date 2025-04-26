#!/bin/bash

set -e

# モデルディレクトリ作成
mkdir -p ${MODEL_DIR}

# モデルがなければダウンロード
if [ ! -f "${MODEL_DIR}/${MODEL_FILE}" ]; then
  echo "モデルが存在しません。ダウンロードを開始します..."
  curl -L "${MODEL_URL}" -o "${MODEL_DIR}/${MODEL_FILE}"
else
  echo "モデルが既に存在します。"
fi

# llama.cpp serverモードで起動
./llama-server -m "${MODEL_DIR}/${MODEL_FILE}" -n 512 --n-gpu-layers 1 --port 8080 --host 0.0.0.0 --ctx-size 4096
