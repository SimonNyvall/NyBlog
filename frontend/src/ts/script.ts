const nextPostButton = document.getElementById('next-post-btn') as HTMLButtonElement;

function updateUrl(newUrl: string) {
  nextPostButton.setAttribute("hx-get", newUrl);
}

let nextPostUrlId = 1;

nextPostButton.addEventListener('click', () => {
  updateUrl(`http://localhost:5021/api/posts/${nextPostUrlId}`);
  console.log(nextPostUrlId);
  nextPostUrlId++;
});
