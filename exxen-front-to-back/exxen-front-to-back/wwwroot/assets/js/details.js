const id=window.location.search.slice(4)
const main=document.querySelector("main")
const body=document.querySelector("body")

fetch(`https://api.tvmaze.com/shows/${id}`)
.then(response=>response.json())
.then(movie=>{
    body.style.backgroundImage=`url(${movie.image.original})`;
    main.innerHTML+=`
    <div class="overlay"></div>
    <div class=" max-w-screen-xl  mx-auto p-4 cus-card overflow-auto w-full sm:w-3/4 md:w-2/3 lg:w-1/2 text-white p-6 bg-black/40 rounded-lg">
    <h1 class="text-3xl font-bold mb-4">${movie.name}</h1>
    <img class="mb-4 rounded" src="${movie.image.medium}" alt="${movie.name}">
    <p class="mb-2" >${movie.genres}</p>
    <p>${movie.summary}</p>

    
    </div>`
})


