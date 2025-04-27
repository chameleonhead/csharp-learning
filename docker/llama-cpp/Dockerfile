FROM ghcr.io/ggml-org/llama.cpp:server

# モデルを置く場所
ENV MODEL_DIR=/models
ENV MODEL_FILE=Llama-3-ELYZA-JP-8B-q4_k_m.gguf
ENV MODEL_URL=https://huggingface.co/elyza/Llama-3-ELYZA-JP-8B-GGUF/resolve/main/Llama-3-ELYZA-JP-8B-q4_k_m.gguf

# 起動時にモデルをダウンロード＆起動するスクリプトを用意
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# モデル保存用ボリューム
VOLUME ["/models"]

# ポート（llama.cppのデフォルト）
EXPOSE 8080

ENTRYPOINT ["/entrypoint.sh"]
