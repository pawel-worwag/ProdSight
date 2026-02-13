#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
API_DIR="src/ProdSight.Api"
DOCKERFILE="$REPO_ROOT/$API_DIR/Dockerfile"

# derive image tag from git when no explicit image name was provided
# priority: exact tag on HEAD -> nearest tag -> short commit SHA
DEFAULT_IMAGE_TAG="local"
if [ -d "$REPO_ROOT/.git" ]; then
  pushd "$REPO_ROOT" >/dev/null
  if TAG=$(git describe --tags --exact-match 2>/dev/null); then
    DEFAULT_IMAGE_TAG="$TAG"
  elif TAG=$(git describe --tags --abbrev=0 2>/dev/null); then
    DEFAULT_IMAGE_TAG="$TAG"
  else
    DEFAULT_IMAGE_TAG="sha-$(git rev-parse --short HEAD)"
  fi
  popd >/dev/null
fi

IMAGE_NAME="${1:-prodsight-api:$DEFAULT_IMAGE_TAG}"
HOST_PORT="${2:-8080}"
CONTAINER_NAME="${3:-prodsight-api-local}"

if [ ! -f "$DOCKERFILE" ]; then
  echo "Dockerfile not found: $DOCKERFILE"
  exit 1
fi

echo "Building Docker image '$IMAGE_NAME' using Dockerfile: $DOCKERFILE"
docker build -t "$IMAGE_NAME" -f "$DOCKERFILE" "$REPO_ROOT"

# Also tag the built image as :latest for the same repository (if applicable)
# e.g. myrepo/myimage:1.2.3 -> myrepo/myimage:latest
IMAGE_REPO="${IMAGE_NAME%%:*}"
if [ -n "$IMAGE_REPO" ]; then
  LATEST_TAG="$IMAGE_REPO:latest"
  echo "Tagging image $IMAGE_NAME as $LATEST_TAG"
  docker tag "$IMAGE_NAME" "$LATEST_TAG"
fi

#echo "Stopping any existing container named '$CONTAINER_NAME'..."
#docker rm -f "$CONTAINER_NAME" 2>/dev/null || true

#echo "Running container '$CONTAINER_NAME' mapping host:$HOST_PORT -> container:80"
#docker run --name "$CONTAINER_NAME" --rm -p "$HOST_PORT:80" "$IMAGE_NAME"

# end
