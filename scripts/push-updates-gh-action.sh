#!/usr/bin/env bash

echo "executing push-updates-gh-action.sh"

echo "::group::cleaning app folder and adding updated files"

rm --recursive ./app
mkdir app
cp --recursive ../artifact/* ./app
chmod --recursive +x ./app

echo "::endgroup::"

echo "::group::listing content of the app folder"
ls ./app
echo "::endgroup::"

echo "::group::udpating workflow to test the new version $LAST_VERSION_RELEASED"

sed -i "s/release\/v[0-9]*.[0-9]*.[0-9]*/release\/$LAST_VERSION_RELEASED/g" .github/workflows/test_gh_action.yml
cat .github/workflows/test_gh_action.yml

echo "::endgroup::"

git config user.name github-actions
git config user.email carlos.angulo.mascarell@outlook.com

echo "::group::git status --short"

git status --short

echo "::endgroup::"

somethingToCommit=$(git status --short)

if [[ -z "$somethingToCommit" ]]; then
    echo "nothing to commit."
else
    echo "::group::git add ."

    git add .

    echo "::endgroup::"

    echo "::group::git commit"

    git commit -m "fix: updated executable files for version $LAST_VERSION_RELEASED"

    echo "::endgroup::"

    git push --set-upstream origin $RELEASE_BRANCH

    echo 'CHANGES_PUSHED=true' >>$GITHUB_ENV
fi
