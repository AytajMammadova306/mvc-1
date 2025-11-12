const row=document.querySelector(".cards")



fetch("https://api.tvmaze.com/shows")
.then(response=>response.json())
.then(movies=>{
    movies.forEach(movie=>{
        row.innerHTML+=`<div
                class="max-w-sm bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
                <a href="details.html?id=${movie.id}">
                    <img class="rounded-t-lg w-full h-64 object-cover" src="${movie.image.medium}" alt="${movie.name}" />
                </a>
                <div class="p-5 mb-2">
                    <a href="details.html?id=${movie.id}">
                        <h5 class="mb-2 text-2xl font-bold tracking-tight text-gray-900 dark:text-white">
                            ${movie.name}
                        </h5>
                    </a>
                    <p class="mb-3 font-normal text-gray-700 dark:text-gray-400">
                        ${movie.rating.average}
                    </p>
                    <a href="details.html?id=${movie.id}" class="cus-text-blacks cus-bg inline-flex items-center px-3 py-2 text-sm font-medium text-center text-white bg-blue-700 rounded-lg
               hover:bg-blue-800
               dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800">
                        Details
                        <svg class="cus-bg rtl:rotate-180 w-3.5 h-3.5 ms-2" aria-hidden="true"
                            xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 14 10">
                            <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M1 5h12m0 0L9 1m4 4L9 9" />
                        </svg>
                    </a>
                </div>
            </div>`
    })
})