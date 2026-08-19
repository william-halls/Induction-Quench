#!/usr/bin/env python3
"""
Gemini-primary chat with automatic Claude escalation on safety blocks.

Chat normally with Gemini. If Gemini refuses to answer (safety block,
recitation block, or an empty/blocked response), this script automatically
packages the conversation context and your question, sends it to Claude
Code (`claude -p`) instead, and shows you Claude's answer in the same
conversation stream.

Usage:
    python gemini_chat.py

Requires GEMINI_API_KEY set in a .env file in the vault root (one directory
up from this script's parent "Claude" folder).
"""

import io
import json
import os
import subprocess
import sys
import urllib.request
import urllib.error

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8")
sys.stdin = io.TextIOWrapper(sys.stdin.buffer, encoding="utf-8")

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
VAULT_ROOT = os.path.dirname(os.path.dirname(SCRIPT_DIR))
ENV_PATH = os.path.join(VAULT_ROOT, ".env")

GEMINI_MODEL = "gemini-flash-latest"
# -1 = dynamic thinking (model decides how much reasoning it needs).
# Set to 0 to disable thinking, or a fixed token count (e.g. 8192) to cap it.
THINKING_BUDGET = -1
GEMINI_URL = (
    f"https://generativelanguage.googleapis.com/v1beta/models/"
    f"{GEMINI_MODEL}:generateContent"
)

BLOCKED_FINISH_REASONS = {"SAFETY", "RECITATION", "BLOCKLIST", "PROHIBITED_CONTENT"}


def load_api_key():
    if not os.path.exists(ENV_PATH):
        sys.exit(f"Missing .env at {ENV_PATH}. Add GEMINI_API_KEY=... to it.")
    with open(ENV_PATH, "r", encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if line.startswith("GEMINI_API_KEY="):
                key = line.split("=", 1)[1].strip()
                if key:
                    return key
    sys.exit("GEMINI_API_KEY not set in .env")


def call_gemini(api_key, history):
    payload = {
        "contents": history,
        "generationConfig": {
            "thinkingConfig": {"thinkingBudget": THINKING_BUDGET}
        },
    }
    body = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        f"{GEMINI_URL}?key={api_key}",
        data=body,
        headers={"Content-Type": "application/json"},
        method="POST",
    )
    try:
        with urllib.request.urlopen(req) as resp:
            return json.loads(resp.read().decode("utf-8")), None, None
    except urllib.error.HTTPError as e:
        body = e.read().decode("utf-8", errors="replace")
        return None, f"HTTP {e.code}: {body}", e.code
    except urllib.error.URLError as e:
        return None, f"Network error: {e}", None


def gemini_is_blocked(data):
    if data is None:
        return True
    prompt_feedback = data.get("promptFeedback", {})
    if prompt_feedback.get("blockReason"):
        return True
    candidates = data.get("candidates", [])
    if not candidates:
        return True
    finish_reason = candidates[0].get("finishReason", "")
    if finish_reason in BLOCKED_FINISH_REASONS:
        return True
    parts = candidates[0].get("content", {}).get("parts", [])
    if not parts or not any(p.get("text", "").strip() for p in parts):
        return True
    return False


def extract_gemini_text(data):
    parts = data["candidates"][0]["content"]["parts"]
    return "".join(p.get("text", "") for p in parts).strip()


def call_claude(history, user_message, block_reason):
    transcript_lines = []
    for turn in history[:-1]:
        speaker = "You" if turn["role"] == "user" else "Assistant"
        text = "".join(p.get("text", "") for p in turn.get("parts", []))
        transcript_lines.append(f"{speaker}: {text}")
    transcript = "\n".join(transcript_lines)

    handoff_prompt = (
        "You are being brought into a conversation that was originally being "
        "handled by Gemini. Gemini hit a safety/content block and could not "
        f"answer the latest question (block reason: {block_reason}). "
        "Please read the prior context and answer the user's question directly "
        "yourself.\n\n"
        f"--- Prior conversation ---\n{transcript if transcript else '(none, this was the first message)'}\n"
        f"--- Question Gemini could not answer ---\n{user_message}\n"
        "--- End context ---\n\n"
        "Answer the question now."
    )

    result = subprocess.run(
        ["claude", "-p", handoff_prompt],
        capture_output=True,
        text=True,
        encoding="utf-8",
    )
    if result.returncode != 0:
        return f"[Claude escalation failed: {result.stderr.strip()}]"
    return result.stdout.strip()


def main():
    api_key = load_api_key()
    history = []

    print("Gemini chat (Claude auto-escalates on safety blocks). Type 'exit' to quit.\n")

    while True:
        try:
            user_message = input("You: ").strip()
        except (EOFError, KeyboardInterrupt):
            print()
            break

        if not user_message:
            continue
        if user_message.lower() in ("exit", "quit"):
            break

        history.append({"role": "user", "parts": [{"text": user_message}]})

        data, error, status_code = call_gemini(api_key, history)

        if error or gemini_is_blocked(data):
            block_reason = error or (
                data.get("promptFeedback", {}).get("blockReason")
                or (data.get("candidates", [{}])[0].get("finishReason") if data and data.get("candidates") else "unknown")
            )
            if status_code == 429:
                print(f"\n[Gemini rate/quota limit hit — escalating to Claude...]\n")
            elif error:
                print(f"\n[Gemini API error ({block_reason}) — escalating to Claude...]\n")
            else:
                print(f"\n[Gemini safety limit hit ({block_reason}) — escalating to Claude...]\n")
            answer = call_claude(history, user_message, block_reason)
            print(f"Claude: {answer}\n")
            history.append({"role": "model", "parts": [{"text": answer}]})
        else:
            answer = extract_gemini_text(data)
            print(f"\nGemini: {answer}\n")
            history.append({"role": "model", "parts": [{"text": answer}]})


if __name__ == "__main__":
    main()
