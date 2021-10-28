function parametros_genetico()
    data = load('parametros_genetico.csv');
    n = 11;

    # Transforma a matriz 121x1 em 11x11
    x = reshape(data(:, 1), n, n);
    y = reshape(data(:, 2), n, n) * 100;
    z = reshape(data(:, 3), n, n);

    max(data(:, 3))

    mesh(x, y, z);
    title("Ganho normalizado (algoritmo genético)", 'FontSize', 12);
    xlabel("População (P)");
    ylabel("Taxa de mutação (Mp)");
    zlabel("Ganho normalizado");
    pause();
endfunction