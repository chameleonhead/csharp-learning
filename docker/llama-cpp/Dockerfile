FROM ghcr.io/ggml-org/llama.cpp:server

# モデルを置く場所
ENV MODEL_DIR=/models
ENV MODEL_FILE=mistral-7b-instruct-v0.2.Q4_K_M.gguf
ENV MODEL_URL=https://huggingface.co/TheBloke/Mistral-7B-Instruct-v0.2-GGUF/resolve/main/mistral-7b-instruct-v0.2.Q4_K_M.gguf

# 起動時にモデルをダウンロード＆起動するスクリプトを用意
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# モデル保存用ボリューム
VOLUME ["/models"]

# ポート（llama.cppのデフォルト）
EXPOSE 8080

ENTRYPOINT ["/entrypoint.sh"]
