#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
API_DIR="src/ProdSight.Api"
DOCKERFILE="$REPO_ROOT/$API_DIR/Dockerfile"

# derive version and default image tag from git
DEFAULT_IMAGE_TAG="local"
VERSION="unspecified"
GIT_SHA="$(git rev-parse --short HEAD 2>/dev/null || echo unknown)"
if [ -d "$REPO_ROOT/.git" ]; then
  pushd "$REPO_ROOT" >/dev/null
  if TAG=$(git describe --tags --exact-match 2>/dev/null); then
    VERSION="$TAG"
  elif TAG=$(git describe --tags --abbrev=0 2>/dev/null); then
    VERSION="$TAG"
  else
    VERSION="0.0.0-$(git rev-parse --short HEAD)"
  fi
  # strip leading 'v' if present
  VERSION="${VERSION#v}"
  DEFAULT_IMAGE_TAG="$VERSION"
  GIT_SHA="$(git rev-parse --short HEAD)"
  popd >/dev/null
fi

# Normalize VERSION to a 4-part numeric AssemblyVersion (major.minor.build.revision)
# Strip pre-release/metadata and non-numeric suffixes
BASE_VERSION="$(echo "$VERSION" | sed -E 's/[-+].*$//')"
IFS='.' read -r -a _parts <<< "$BASE_VERSION"
for i in 0 1 2 3; do
  part="${_parts[i]:-0}"
  # keep only leading digits
  part="$(echo "$part" | sed -E 's/^([0-9]+).*/\1/; t; s/.*/0/')"
  eval "v$i=\$part"
done
ASSEMBLY_VERSION="$v0.$v1.$v2.$v3"

DOCKER_BUILD_OPTS=""
# Filter arguments: remove --no-cache from positional args and set DOCKER_BUILD_OPTS when present
ARGS=()
for a in "$@"; do
  if [ "$a" = "--no-cache" ]; then
    DOCKER_BUILD_OPTS="--no-cache"
  else
    ARGS+=("$a")
  fi
done
# If NO_CACHE env var set, enable no-cache
if [ "${NO_CACHE:-}" = "1" ] || [ "${NO_CACHE:-}" = "true" ]; then
  DOCKER_BUILD_OPTS="--no-cache"
fi
set -- "${ARGS[@]}"

IMAGE_NAME="${1:-prodsight-api:$DEFAULT_IMAGE_TAG}"
HOST_PORT="${2:-8080}"
CONTAINER_NAME="${3:-prodsight-api-local}"

if [ ! -f "$DOCKERFILE" ]; then
  echo "Dockerfile not found: $DOCKERFILE"
  exit 1
fi

echo "Building Docker image '$IMAGE_NAME' using Dockerfile: $DOCKERFILE"
echo "docker build $DOCKER_BUILD_OPTS --build-arg VERSION=\"$VERSION\" --build-arg GIT_SHA=\"$GIT_SHA\" --build-arg ASSEMBLY_VERSION=\"$ASSEMBLY_VERSION\" -t \"$IMAGE_NAME\" -f \"$DOCKERFILE\" \"$REPO_ROOT\""
docker build $DOCKER_BUILD_OPTS \
  --build-arg VERSION="$VERSION" \
  --build-arg GIT_SHA="$GIT_SHA" \
  --build-arg ASSEMBLY_VERSION="$ASSEMBLY_VERSION" \
  -t "$IMAGE_NAME" -f "$DOCKERFILE" "$REPO_ROOT"

# Also tag the built image as :latest for the same repository (if applicable)
# e.g. myrepo/myimage:1.2.3 -> myrepo/myimage:latest
IMAGE_REPO="${IMAGE_NAME%%:*}"
if [ -n "$IMAGE_REPO" ]; then
  SHA_TAG="$IMAGE_REPO:sha-$GIT_SHA"
  LATEST_TAG="$IMAGE_REPO:latest"
  echo "Tagging image $IMAGE_NAME as $SHA_TAG and $LATEST_TAG"
  docker tag "$IMAGE_NAME" "$SHA_TAG"
  docker tag "$IMAGE_NAME" "$LATEST_TAG"
fi

#echo "Stopping any existing container named '$CONTAINER_NAME'..."
#docker rm -f "$CONTAINER_NAME" 2>/dev/null || true

#echo "Running container '$CONTAINER_NAME' mapping host:$HOST_PORT -> container:80"
#docker run --name "$CONTAINER_NAME" --rm -p "$HOST_PORT:80" "$IMAGE_NAME"

# end
